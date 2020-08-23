using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IFBMVVM.Model
{
    public class Model_ImageFiles : INotifyPropertyChanged
    {
        string _Filefullname = string.Empty;
        public string Filefullname
        {
            get { return this._Filefullname; }
            set
            {
                if (this._Filefullname != value)
                {
                    this._Filefullname = value;
                    NotifyPropertyChanged("Filefullname");
                }
            }
        }

        Model_ImageExIfProperties _exifprop = new Model_ImageExIfProperties();
        public Model_ImageExIfProperties ImageEXIFProperties
        {
            get { return this._exifprop; }
            set
            {
                if (this._exifprop != value)
                {
                    this._exifprop = value;
                    NotifyPropertyChanged("ImageEXIFProperties");
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
