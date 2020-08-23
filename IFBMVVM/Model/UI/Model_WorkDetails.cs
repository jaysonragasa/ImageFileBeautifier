using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using IFBMVVM.Common.Enums;
using System.Windows.Media;

namespace IFBMVVM.Model.UI
{
    public class Model_WorkDetails : INotifyPropertyChanged
    {
        string _SourcePath = string.Empty;
        public string SourcePath
        {
            get { return this._SourcePath; }
            set
            {
                if (this._SourcePath != value)
                {
                    this._SourcePath = value;
                    NotifyPropertyChanged("SourcePath");
                }
            }
        }

        private bool _ScanSubdirectories = true;
        public bool ScanSubdirectories
        {
            get { return _ScanSubdirectories; }
            set
            {
                if (_ScanSubdirectories != value)
                {
                    _ScanSubdirectories = value;
                    NotifyPropertyChanged("ScanSubdirectories");
                }
            }
        }

        string _Destination = string.Empty;
        public string Destination
        {
            get { return this._Destination; }
            set
            {
                if (this._Destination != value)
                {
                    this._Destination = value;
                    NotifyPropertyChanged("Destination");
                }
            }
        }

        //TODO_WHEN_DUPLICATE _todo_when_dup = TODO_WHEN_DUPLICATE.NOTHING;
        //public TODO_WHEN_DUPLICATE TodoWhenDuplicate
        //{
        //    get { return this._todo_when_dup; }
        //    set
        //    {
        //        if (this._todo_when_dup != value)
        //        {
        //            this._todo_when_dup = value;
        //            NotifyPropertyChanged("TodoWhenDuplicate");
        //        }
        //    }
        //}

        int _ProgressBarMaximum = 10;
        public int ProgressBarMaximum
        {
            get { return this._ProgressBarMaximum; }
            set
            {
                if (this._ProgressBarMaximum != value)
                {
                    this._ProgressBarMaximum = value;
                    NotifyPropertyChanged("ProgressBarMaximum");
                }
            }
        }

        int _ProgressBarValue = 10;
        public int ProgressBarValue
        {
            get { return this._ProgressBarValue; }
            set
            {
                if (this._ProgressBarValue != value)
                {
                    this._ProgressBarValue = value;
                    NotifyPropertyChanged("ProgressBarValue");
                }
            }
        }

        SolidColorBrush _ItemBackground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        public SolidColorBrush ItemBackground
        {
            get { return this._ItemBackground; }
            set
            {
                if (this._ItemBackground != value)
                {
                    this._ItemBackground = value;
                    NotifyPropertyChanged("ItemBackground");
                }
            }
        }

        bool _isBusy = false;
        public bool IsBusy
        {
            get { return this._isBusy; }
            set
            {
                if (this._isBusy != value)
                {
                    this._isBusy = value;
                    NotifyPropertyChanged("IsBusy");
                }
            }
        }

        bool _done = false;
        public bool WorkDone
        {
            get { return this._done; }
            set
            {
                if (this._done != value)
                {
                    this._done = value;
                    NotifyPropertyChanged("WorkDone");
                }
            }
        }

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

        //string _yearPrefix = "Year";
        //public string YearPrefix
        //{
        //    get { return this._yearPrefix; }
        //    set { this._yearPrefix = value; }
        //}

        //string _monthPrefix = "Month of";
        //public string MonthPrefix
        //{
        //    get { return this._monthPrefix; }
        //    set { this._monthPrefix = value; }
        //}

        //string _filenameformat = "MMMM dd, yyyy - hhmmss";
        //public string FileNameFormat
        //{
        //    get { return this._filenameformat; }
        //    set { this._filenameformat = value; }
        //}

        bool _useDateModified = false;
        public bool UseDateModified
        {
            get { return this._useDateModified; }
            set
            {
                if (this._useDateModified != value)
                {
                    this._useDateModified = value;
                    NotifyPropertyChanged("UseDateModified");
                }
            }
        }

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
