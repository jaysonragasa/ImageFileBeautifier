using System;
using System.Net;
using System.Management;
using System.Security.Cryptography;
using System.Security;
using System.Collections;
using System.Text;
using System.Diagnostics;

namespace LicenseKey
{
    public class Licensing
    {
        string baseURL = "";
        string localURL = "http://localhost:70/shop/";
        string prodcURL = "http://shop.jaysonragasa.net/";

        public Licensing()
        {
            baseURL = prodcURL;
        }
        static readonly Licensing _i = new Licensing();
        public static Licensing I { get { return _i; } }

        public bool ValidateLicenseKey(string licensekey, string email)
        {
            bool ret = false;

            string lid = licensekey;
            string mid = FingerPrint.Value();
            string pid = "1";
            string eml = email;

            using (WebClient web = new WebClient()
            {
                Proxy = null
            })
            {
                // http://shop.jaysonragasa.net/machinecheck.php?pid=1&lid=ryantuason&mid=1&email=jayson.ragasa@live.com
                string request = string.Format(baseURL + "validate.php?pid={0}&lid={1}&mid={2}&email={3}", pid, lid, mid, eml);

                try
                {
                    string response = web.DownloadString(request);

                    Debug.WriteLine(response);

                    if (response.ToLower() == "valid")
                    {
                        ret = true;
                    }
                }
                catch (WebException wex)
                {
                    throw new WebException(wex.Message, wex);
                }
            }

            return ret;
        }

        public bool MachineCheck(string licensekey, string email, string machineid = "")
        {
            bool ret = false;

            string lid = licensekey;
            string mid = machineid == string.Empty ? FingerPrint.Value() : machineid;
            string pid = "1";
            string eml = email;

            using (WebClient web = new WebClient()
            {
                Proxy = null
            })
            {
                // http://shop.jaysonragasa.net/machinecheck.php?pid=1&lid=ryantuason&mid=1&email=jayson.ragasa@live.com
                string request = string.Format(baseURL + "machinecheck.php?pid={0}&lid={1}&mid={2}&email={3}", pid, lid, mid, eml);

                try
                {
                    string response = web.DownloadString(request);

                    Debug.WriteLine(response);

                    if (response.ToLower() == "valid")
                    {
                        ret = true;
                    }
                }
                catch (WebException wex)
                {
                    throw new WebException(wex.Message, wex);
                }
            }

            return ret;
        }
    }

    /// <summary>
    /// Generates a 16 byte Unique Identification code of a computer
    /// Example: 4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9
    /// </summary>
    public class FingerPrint
    {
        private static string fingerPrint = string.Empty;
        public static string Value()
        {
            if (string.IsNullOrEmpty(fingerPrint))
            {
                string[] fingerprint = {
                                           "CPU=" + cpuId(),
                                           "BIOS=" + biosId(),
                                           //"BASE=" + baseId(),
                                           //"DISK=" + diskId(),
                                           //"VIDEO=" + videoId(),
                                           "MAC="+ macId()
                                       };

                fingerPrint = string.Join("\r\n", fingerprint);
                fingerPrint = GetHash(fingerPrint);
            }
            return fingerPrint;
        }
        private static string GetHash(string s)
        {
            MD5 sec = new MD5CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] bt = enc.GetBytes(s);
            return GetHexString(sec.ComputeHash(bt));
        }
        private static string GetHexString(byte[] bt)
        {
            string s = string.Empty;
            for (int i = 0; i < bt.Length; i++)
            {
                byte b = bt[i];
                int n, n1, n2;
                n = (int)b;
                n1 = n & 15;
                n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + (int)'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char)(n1 - 10 + (int)'A')).ToString();
                else
                    s += n1.ToString();
                if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "-";
            }
            return s;
        }
        #region Original Device ID Getting Code
        //Return a hardware identifier
        private static string identifier
        (string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string result = "";
            System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() == "True")
                {
                    //Only get the first one
                    if (result == "")
                    {
                        try
                        {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return result;
        }
        //Return a hardware identifier
        private static string identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            System.Management.ManagementClass mc = new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                //Only get the first one
                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }
        private static string cpuId()
        {
            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as it is very time consuming
            string retVal = identifier("Win32_Processor", "UniqueId");
            if (retVal == "") //If no UniqueID, use ProcessorID
            {
                retVal = identifier("Win32_Processor", "ProcessorId");
                if (retVal == "") //If no ProcessorId, use Name
                {
                    retVal = identifier("Win32_Processor", "Name");
                    if (retVal == "") //If no Name, use Manufacturer
                    {
                        retVal = identifier("Win32_Processor", "Manufacturer");
                    }
                    //Add clock speed for extra security
                    retVal += identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return retVal;
        }
        //BIOS Identifier
        private static string biosId()
        {
            return identifier("Win32_BIOS", "Manufacturer")
            + identifier("Win32_BIOS", "SMBIOSBIOSVersion")
            + identifier("Win32_BIOS", "IdentificationCode")
            + identifier("Win32_BIOS", "SerialNumber")
            + identifier("Win32_BIOS", "ReleaseDate")
            + identifier("Win32_BIOS", "Version");
        }
        //Main physical hard drive ID
        private static string diskId()
        {
            return identifier("Win32_DiskDrive", "Model")
            + identifier("Win32_DiskDrive", "Manufacturer")
            + identifier("Win32_DiskDrive", "Signature")
            + identifier("Win32_DiskDrive", "TotalHeads");
        }
        //Motherboard ID
        private static string baseId()
        {
            return identifier("Win32_BaseBoard", "Model")
            + identifier("Win32_BaseBoard", "Manufacturer")
            + identifier("Win32_BaseBoard", "Name")
            + identifier("Win32_BaseBoard", "SerialNumber");
        }
        //Primary video controller ID
        private static string videoId()
        {
            return identifier("Win32_VideoController", "DriverVersion")
            + identifier("Win32_VideoController", "Name");
        }
        //First enabled network card ID
        private static string macId()
        {
            return identifier("Win32_NetworkAdapterConfiguration",
                "MACAddress", "IPEnabled");
        }
        #endregion
    }
}
