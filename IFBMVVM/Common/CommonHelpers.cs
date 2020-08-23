using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace IFBMVVM.Common
{
    public class CommonHelpers
    {
        public CommonHelpers()
        {
        }
        static readonly CommonHelpers _i = new CommonHelpers();
        public static CommonHelpers Instance { get { return _i; } }

        public string GetUniqueKey(int maxSize = 8, int minSize = 5)
        {
            //int maxSize = 4;
            //int minSize = 3;

            int size = maxSize;

            char[] chars = new char[62];
            byte[] data = new byte[1];

            //string a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string a = "1234567890";
            chars = a.ToCharArray();

            StringBuilder result = new StringBuilder();

            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                size = maxSize;
                data = new byte[size];
                crypto.GetNonZeroBytes(data);

                result = new StringBuilder(size);
            }
            
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);
            }

            return result.ToString();
        }
    }
}
