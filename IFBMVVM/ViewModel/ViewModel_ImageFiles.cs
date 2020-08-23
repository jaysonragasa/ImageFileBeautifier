using IFBMVVM.Common.Imaging.EXIF.GoheerLib;
using IFBMVVM.Common.IO;
using IFBMVVM.Interface;
using IFBMVVM.Interface.UI;
using IFBMVVM.Model;
using IFBMVVM.Model.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace IFBMVVM.ViewModel
{
    public delegate void DoneHandler(object sender, Model_WorkDetails WorkDetails);
    public class ViewModel_ImageFiles : INotifyPropertyChanged 
    {
        iMainWindow _view;
        iNamingFormat _namFormat;

        iStatus _stat;
        Queue<string> Files;
        BackgroundWorker exifReadWorker;
        int count = 0;
        public event DoneHandler Done;
        FileFolder _filefolder;

        #region // Properties /
        string _WorkName = string.Empty;
        public string WorkName
        {
            get { return this._WorkName; }
            set
            {
                if (this._WorkName != value)
                {
                    this._WorkName = value;
                    NotifyPropertyChanged("WorkName");
                }
            }
        }

        List<string> _NoEXIF = new List<string>();
        public List<string> NoEXIF
        {
            get { return this._NoEXIF; }
            set
            {
                this._NoEXIF = value;
                NotifyPropertyChanged("NoEXIF");
            }
        }

        List<string> _NotMoved = new List<string>();
        public List<string> NotMoved
        {
            get { return this._NotMoved; }
            set
            {
                this._NotMoved = value;
                NotifyPropertyChanged("NotMoved");
            }
        }

        ObservableCollection<Model_ImageFiles> _exprop = new ObservableCollection<Model_ImageFiles>();
        public ObservableCollection<Model_ImageFiles> ImageFileCollections
        {
            get { return this._exprop; }
            set
            {
                this._exprop = value;
                NotifyPropertyChanged("ImageFileCollections");
            }
        }

        Model_WorkDetails _workdetails = new Model_WorkDetails();
        public Model_WorkDetails WorkDetails
        {
            get { return this._workdetails; }
            set
            {
                this._workdetails = value;
                NotifyPropertyChanged("WorkDetails");
            }
        }

        bool _recuse_subfolder = false;
        public bool RecurseSubfolder
        {
            get { return this._recuse_subfolder; }
            set { this._recuse_subfolder = value; }
        }
        #endregion

        public ViewModel_ImageFiles(iMainWindow view, iNamingFormat namFormat)
        {
            this._view = view;
            this._namFormat = namFormat;

            Initialize();
        }

        public ViewModel_ImageFiles(iNamingFormat namFormat)
        {
            this._namFormat = namFormat;
            Initialize();
        }

        void Initialize()
        {
            this.exifReadWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            this.exifReadWorker.DoWork += exifReadWorker_DoWork;
            this.exifReadWorker.ProgressChanged += exifReadWorker_ProgressChanged;
            this.exifReadWorker.RunWorkerCompleted += exifReadWorker_RunWorkerCompleted;

            this._filefolder = new FileFolder(this._namFormat);
        }

        #region // Events /

        void exifReadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProcessImageFile();
        }

        void exifReadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.WorkDetails.ProgressBarValue = count;
            this._stat.RenamedFiles++;
        }

        void exifReadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this._stat.TrialOver) { return; }

            string current_file = e.Argument.ToString();

            FileInfo fi = new FileInfo(current_file);
            if (fi.Length == 0) return;

            Model_ImageExIfProperties Properties = new Model_ImageExIfProperties();
            Regex r = new Regex(":");

            // retreive EXIF of image file
            try
            {
                //Properties = GetExIf(current_file);

                using (FileStream fs = new FileStream(current_file, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    PropertyItem propItem = myImage.GetPropertyItem(36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);

                    Properties.EXIF_DTOrig = dateTaken;
                }
            }
            catch (Exception ex)
            {
                this._stat.LogText = "ERROR in exifReadWorker_DoWork(" + current_file + ") while getting EXIF\r\n" + ex.Message + "\r\n" + ex.StackTrace;
            }

            string DT = string.Empty;

            if (string.IsNullOrEmpty(Properties.EXIF_DTOrig)) // if DTOrig is null, which is very important
            {
                if (this._workdetails.UseDateModified)
                {
                    DT = fi.LastWriteTime.ToString();
                }
                else
                {
                    this.NoEXIF.Add(current_file);

                    count++;
                    this.exifReadWorker.ReportProgress(count);

                    // do not continue
                    return;
                }
            }
            else
            {
                DT = Properties.EXIF_DTOrig;
            }

            // move to output folder
            try
            {
                DateTime dt = DateTime.Parse(DT, null, System.Globalization.DateTimeStyles.AssumeLocal);
                string newfilename = this._filefolder.MoveFileToProperFolder(current_file, dt, dt.ToString(this._namFormat.FileNameFormat));

                // newfilename is null, it means the file has a duplicate in the destination folder
                if (string.IsNullOrEmpty(newfilename))
                {
                    this._stat.LogText = "ERROR in exifReadWorker_DoWork(" + newfilename + ") while moving the file\r\nDUPLICATE";
                    this.NotMoved.Add(current_file);
                }
            }
            catch (Exception ex)
            {
                this._stat.LogText = "ERROR in exifReadWorker_DoWork(" + current_file + ") while moving the file\r\n" + ex.Message + "\r\n" + ex.StackTrace;
                this.NotMoved.Add(current_file);
            }

            count++;
            this.exifReadWorker.ReportProgress(count);
        }

        #endregion

        #region // Methods /

        public void InitStat(iStatus stat)
        {
            _stat = stat;
        }

        public void ScanFiles()
        {
            this.Files = new Queue<string>();

            if (Directory.Exists(this.WorkDetails.SourcePath))
            {
                this._filefolder.ScanFiles(this.WorkDetails.SourcePath, this.WorkDetails.ScanSubdirectories).ForEach((x) =>
                {
                    FileInfo fi = new FileInfo(x);
                    if (fi.Exists)
                    {
                        // accept JPG only file
                        if (fi.Extension.ToLower() == ".jpg")
                        {
                            // enqueue files
                            this.Files.Enqueue(fi.FullName);
                        }
                    }
                });
            }
            else
            {
                this.Files.Enqueue(this.WorkDetails.SourcePath);
            }

            // set output directory
            this._filefolder.OutputDirectory = this.WorkDetails.Destination;
            //if (Directory.Exists(this.WorkDetails.SourcePath))
            //{
            //    this._filefolder.OutputDirectory = this.WorkDetails.Destination;
            //}
            //else
            //{
            //    this._filefolder.OutputDirectory = this.WorkDetails.Destination;
            //}
            //this._filefolder.YearPrefix = this.WorkDetails.YearPrefix;
            //this._filefolder.MonthPrefix = this.WorkDetails.MonthPrefix;
            //this._filefolder.TodoWhenDuplicate = this.WorkDetails.TodoWhenDuplicate;

            // reset progress bar
            this.WorkDetails.ProgressBarMaximum = this.Files.Count;
            this.WorkDetails.ProgressBarValue = 0;

            // reset collections
            this.ImageFileCollections.Clear();
            this.NoEXIF.Clear();
            this.NotMoved.Clear();

            this.WorkDetails.IsBusy = true;
            this.WorkDetails.WorkDone = false;
            this.WorkDetails.WorkName = this.WorkName;

            // start processing the images
            ProcessImageFile();
        }

        void ProcessImageFile()
        {
            if (this.Files.Any())
            {
                this.exifReadWorker.RunWorkerAsync(this.Files.Dequeue());
            }
            else
            {
                if ((this.NotMoved.Count + this.NoEXIF.Count) > 0)
                {
                    // todo: some errors
                }

                if (Done != null)
                {
                    this.Done(this, this._workdetails);
                }

                this.WorkDetails.IsBusy = false;
                this.WorkDetails.WorkDone = true;
            }
        }

        public Model_ImageExIfProperties GetExIf(string file)
        {
            Model_ImageExIfProperties ret = new Model_ImageExIfProperties();
            ExIfExtractor ef = new ExIfExtractor(file, string.Empty, string.Empty);

            foreach (PropertyInfo pi in ret.GetType().GetProperties())
            {
                string propname = pi.Name;

                if (propname.IndexOf("EXIF_") == 0)
                {
                    propname = propname.Replace("EXIF_", string.Empty);
                    pi.SetValue(ret, Convert.ChangeType(ef[propname], pi.PropertyType), null);
                }
            }

            return ret;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed. 
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion 
    }
}
