using IFBMVVM.Common.Enums;
using IFBMVVM.Interface;
using System;
using System.Collections.Generic;
using System.IO;

namespace IFBMVVM.Common.IO
{
    public class FileFolder
    {
        List<string> _allowedExt = new List<string>()
        {
            ".jpg"
#if EXP_ALLOW_VIDEOS
            , ".mp4", ".mov", ".avi", ".mpg"
#else
#endif
        };

        INamingFormat namFormat;

#region // Properties /

        public string OutputDirectory { get; set; }
        public string SourceDirectory { get; set; }
        public bool RecurseDirectory { get; set; }
        public bool ScanSubidrectory { get; set; }

        string _monthfolderformat = "MMMM";
        public string MonthFolderFormat
        {
            get { return this._monthfolderformat; }
            set { this._monthfolderformat = value; }
        }

#endregion

#region // Constructor /

        public FileFolder(INamingFormat _namFormat)
        {
            this.namFormat = _namFormat;
            this.ScanSubidrectory = true;
        }

        public FileFolder()
        {
        }
        static readonly FileFolder _i = new FileFolder();
        public static FileFolder Instance { get { return _i; } }

#endregion

        public bool isAccessible(string folder)
        {
            bool res = true;

            try
            {
                string[] files = Directory.GetFiles(folder);
            }
            catch (Exception ex)
            {
                res = false;
            }

            return res;
        }

        public List<string> ScanFiles(string Source, bool ScanSubdirectories)
        {
            this.SourceDirectory = Source;
            this.ScanSubidrectory = ScanSubdirectories;

            return GetFilesRecursive(this.SourceDirectory);
        }

        List<string> GetFilesRecursive(string b, bool includeSubFolder)
        {
            List<string> ret = new List<string>();

            if (includeSubFolder)
            {
                this.ScanSubidrectory = true;
            }
            else
            {
                this.ScanSubidrectory = false;
            }

            ret = GetFilesRecursive(b);

            return ret;
        }

        List<string> GetFilesRecursive(string b)
        {
            List<string> result = new List<string>();
            Stack<string> stack = new Stack<string>();

            stack.Push(b);

            while (stack.Count > 0)
            {
                string dir = stack.Pop();

                try
                {
                    //result.AddRange(Directory.GetFiles(dir, "*.*"));

                    if (this.ScanSubidrectory)
                    {
                        foreach (string dn in Directory.GetDirectories(dir))
                        {
                            stack.Push(dn);
                        }
                    }

                    foreach (string fn in Directory.GetFiles(dir, "*.*"))
                    {
                        FileInfo fi = new FileInfo(fn);
                        if (_allowedExt.Count == 0)
                        {
                            result.Add(fi.FullName);
                        }
                        else
                        {
                            if (_allowedExt.Contains(fi.Extension.ToLowerInvariant()))
                            {
                                result.Add(fi.FullName);
                            }
                        }
                    }
                }
                catch
                {
                    ;
                }
            }

            return result;
        }

        public string RenameFile(string org, string newname)
        {
            FileInfo fi = new FileInfo(org);
            string newfilename = Path.Combine(fi.Directory.FullName, newname + fi.Extension);

            if (!fi.Exists)
            {
                fi.MoveTo(newfilename);
            }
            else
            {
                newfilename = null;
            }

            return newfilename;
        }

        public string MoveFileToProperFolder(string fullname, DateTime exif_DateCreated, string newname)
        {
            FileInfo fi = new FileInfo(fullname);

            string currentDir = this.OutputDirectory; //fi.Directory.FullName;
            string year = string.Format("{0} {1}", this.namFormat.YearPrefix, exif_DateCreated.Year.ToString());
            year = Path.Combine(currentDir, year);
            string month = string.Format("{0} {1}", this.namFormat.MonthPrefix, exif_DateCreated.ToString(MonthFolderFormat));
            month = Path.Combine(year, month);

            // create directories
            if (!Directory.Exists(year))
            {
                Directory.CreateDirectory(year);
            }

            if (!Directory.Exists(month))
            {
                Directory.CreateDirectory(month);
            }

            // new filename
            string fullpath = Path.Combine(month, newname + fi.Extension);

            if (this.namFormat.TodoWhenDuplicate == TODO_WHEN_DUPLICATE.NOTHING)
            {
                if (!File.Exists(fullpath))
                {
                    fi.MoveTo(fullpath);
                }
                else
                {
                    fullpath = null;
                }
                
            }
            else if (this.namFormat.TodoWhenDuplicate == TODO_WHEN_DUPLICATE.ADD_UNIQUE_NUMBER)
            {
                string name = newname;
                name += " - " + CommonHelpers.Instance.GetUniqueKey(4, 3);
                fullpath = Path.Combine(month, name + fi.Extension);

                fi.MoveTo(fullpath);
            }

            return fullpath;
        }
    }
}
