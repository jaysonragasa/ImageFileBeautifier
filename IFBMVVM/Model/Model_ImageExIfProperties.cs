using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IFBMVVM.Model
{
    public class Model_ImageExIfProperties : INotifyPropertyChanged
    {
        string _PixXDim = string.Empty;
        public string EXIF_PixXDim
        {
            get { return _PixXDim; }
            set
            {
                if (_PixXDim != value)
                {
                    _PixXDim = value;
                    //NotifyPropertyChanged("EXIF_PixXDim");
                }
            }
        }

        string _PixYDim = string.Empty;
        public string EXIF_PixYDim
        {
            get { return this._PixYDim; }
            set
            {
                if (this._PixYDim != value)
                {
                    this._PixYDim = value;
                    //NotifyPropertyChanged("EXIF_PixYDim");
                }
            }
        }

        string _DTOrig = string.Empty;
        public string EXIF_DTOrig
        {
            get { return this._DTOrig; }
            set
            {
                if (this._DTOrig != value && !string.IsNullOrEmpty(value))
                {
                    string raw = value;
                    raw = raw.Replace("\0", string.Empty);
                    // 2017-04-01 22:05:52
                    string newDate = DateTime.ParseExact(raw, "yyyy-MM-dd HH:mm:ss", null).ToString();
                    this._DTOrig = newDate;
                    //NotifyPropertyChanged("EXIF_DTOrig");
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
