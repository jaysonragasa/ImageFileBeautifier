using IFBMVVM.Common;
using IFBMVVM.Interface;
using IFBMVVM.Interface.UI;
using IFBMVVM.Model.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IFBMVVM.ViewModel.UI
{
    public class ViewModel_MainWindow : INotifyPropertyChanged //, iStatus
    {
        iMainWindow _view;
        iNamingFormat _namFormat;
        ViewModel_ImageFiles _vmif;
        Timer tmr = new Timer();
        iStatus _stat;

        ObservableCollection<Model_WorkDetails> _SourceCollection = new ObservableCollection<Model_WorkDetails>();
        public ObservableCollection<Model_WorkDetails> SourceCollections
        {
            get { return this._SourceCollection; }
            set
            {
                this._SourceCollection = value;
                NotifyPropertyChanged("SourceCollections");
            }
        }

        public ViewModel_MainWindow(iMainWindow view, iNamingFormat namFormat)
        {
            this._view = view;
            this._namFormat = namFormat;

            this.tmr = new Timer()
            {
                Interval = 1000,
                Enabled = true
            };
            tmr.Tick += tmr_Tick;
        }

        public void InitStat(iStatus stat)
        {
            this._stat = stat;
        }

        void tmr_Tick(object sender, EventArgs e)
        {
            if (SourceCollections.Any())
            {
                var a = SourceCollections.Where(x => x.IsBusy == false && x.WorkDone == false);
                if (a != null && a.Count() > 0)
                {
                    Model_WorkDetails work = a.First();
                    ViewModel_ImageFiles vmif = new ViewModel_ImageFiles(_namFormat);
                    vmif.WorkDetails = work;
                    vmif.WorkName = work.SourcePath;
                    vmif.Done += vmif_Done;
                    vmif.InitStat(_stat);
                    vmif.ScanFiles();
                }
            }
            else
            {

            }
        }

        void vmif_Done(object sender, Model_WorkDetails e)
        {
            //var a = SourceCollections.Pop();
            Model_WorkDetails wd = SourceCollections.Where(x => x.WorkName == e.WorkName).First();
            if (wd != null)
            {
                SourceCollections.Remove(wd);
            }
        }

        void _vmif_Done(object sender, EventArgs e)
        {
            
        }

        public void EnqueueNewSource(Model_WorkDetails workDetails)
        {
            // make sure what we are Pushing does not exists
            int a = SourceCollections.Where(x => x.SourcePath == workDetails.SourcePath).ToList().Count;

            if (a == 0)
            {
                //SourceCollections.Push(workDetails);

                SourceCollections.Add(workDetails);
            }
        }

        public string OpenFolderDialog()
        {
            string selFolder = string.Empty;

            FolderBrowserDialog fbd = new FolderBrowserDialog()
            {
                Description = "Open image folder"
            };
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                selFolder = fbd.SelectedPath;
            }

            return selFolder;
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

        #region iStatus imple
        //string _logtext = string.Empty;
        //public string LogText
        //{
        //    get { return this._logtext; }
        //    set
        //    {
        //        this._logtext = value;
        //        Status.Instance.LogText(value);
        //    }
        //}

        //int _renamedfiles = 0;
        //public int RenamedFiles
        //{
        //    get
        //    {
        //        return _renamedfiles;
        //    }
        //    set
        //    {
        //        this._renamedfiles = value;
        //    }
        //}

        //bool _trialover = false;
        //public bool TrialOver
        //{
        //    get
        //    {
        //        return _trialover;
        //    }
        //    set { _trialover = value; }
        //}
        #endregion
    }
}
