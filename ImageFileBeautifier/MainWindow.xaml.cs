using IFBMVVM.Common.Enums;
using IFBMVVM.Common.IO;
using IFBMVVM.Interface;
using IFBMVVM.Interface.UI;
using IFBMVVM.ViewModel.UI;
using LicenseKey;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageFileBeautifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, iMainWindow, iStatus, iNamingFormat
    {
        ViewModel_MainWindow vmme;
        Random r = new Random(DateTime.Now.Second);

        public MainWindow()
        {
            InitializeComponent();
            InitializeControlEvents();
            InitializeObjects();
            InitializeControls();
        }

        #region // Initializers /

        void InitializeObjects()
        {
            this.vmme = new ViewModel_MainWindow(this, this);
            this.vmme.InitStat(this);
            listWork.ItemsSource = this.vmme.SourceCollections;
        }

        void InitializeControls()
        {
            
        }

        void InitializeControlEvents()
        {
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            this.txYearFolderPrefix.TextChanged += txYearFolderPrefix_TextChanged;
            this.txMonthFolderPrefix.TextChanged += txMonthFolderPrefix_TextChanged;
            this.cbFileNameFormats.SelectionChanged += cbFileNameFormats_SelectionChanged;
            this.btnOpenFolder.MouseLeftButtonDown += btnOpenFolder_MouseLeftButtonDown;
            this.btnOpenFolder_hover.PreviewMouseLeftButtonUp += btnOpenFolder_hover_PreviewMouseLeftButtonUp;
        }

        #endregion

        #region // Events /

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            VisualState(EnumVisualState.Page1);

            LoadSettings();
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
        }

        private void Grid_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        void vmif_Done(object sender, EventArgs e)
        {
            VisualState(EnumVisualState.BeautifyingDone);
            Properties.Settings.Default.Save();
        }

        private void panelOpenFolderIcon_DragEnter(object sender, DragEventArgs e)
        {
            string[] _path = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            Debug.WriteLine(_path.First());
            if (Directory.Exists(_path.First()))
            {
                VisualState(EnumVisualState.DragFolder);
            }
            else if (File.Exists(_path.First()))
            {
                VisualState(EnumVisualState.DragFolder);
            }
            else
            {
                VisualState(EnumVisualState.DragNotFolder);
            }
        }

        private void panelOpenFolderIcon_Drop(object sender, DragEventArgs e)
        {
            string[] _path = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string _dirpath = _path.First();

            if (Directory.Exists(_dirpath))
            {
                StartScan(_dirpath, _dirpath);
            }
            else if (File.Exists(_dirpath))
            {
                foreach (string file in _path)
                {
                    StartScan(file, _dirpath);
                }
            }
        }

        private void btnSettings_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            VisualState(EnumVisualState.ShowSettings);
        }

        private void panelOpenFolderIcon_DragLeave(object sender, DragEventArgs e)
        {
            VisualState(EnumVisualState.DragNotFolder);
        }

        #region sysbuttons
        private void btnMinimize_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void btnHelp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            VisualState(EnumVisualState.ShowAbout);
        }

        private void btnClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void sysmenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
        }
        #endregion

        private void btnCloseAbout_Click(object sender, RoutedEventArgs e)
        {
            VisualState(EnumVisualState.HideAbout);
        }

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            VisualState(EnumVisualState.HideSettings);
        }

        private void btnCloseSettings_Click(object sender, RoutedEventArgs e)
        {
            VisualState(EnumVisualState.HideSettings);
        }

        #region preview
        private void txYearFolderPrefix_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblYearPreview.Text = "ex: " + txYearFolderPrefix.Text + " " + DateTime.Today.Year;
        }

        private void txMonthFolderPrefix_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblMonthPreview.Text = "ex: " + txMonthFolderPrefix.Text + " " + DateTime.Today.Month;
        }

        private void cbFileNameFormats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblFileNamePreview.Text = "ex: " + DateTime.Now.ToString(cbFileNameFormats.SelectedItem.ToString());
        }
        #endregion

        void btnOpenFolder_hover_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            VisualState(EnumVisualState.DragNotFolder);

            string selected_folder = this.vmme.OpenFolderDialog();
            if (!string.IsNullOrEmpty(selected_folder))
            {
                StartScan(selected_folder, selected_folder);
            }
        }

        void btnOpenFolder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VisualState(EnumVisualState.DragFolder);
        }

        private void btnShowAbout_Click(object sender, RoutedEventArgs e)
        {
            VisualState(EnumVisualState.ShowAboutGrid);
        }

        private void btnShowHelp_Click(object sender, RoutedEventArgs e)
        {
            VisualState(EnumVisualState.ShowHelpGrid);
        }

        private void btnShowPrivacyStatement_Click(object sender, RoutedEventArgs e)
        {
            VisualState(EnumVisualState.ShowPrivacyGrid);
        }

        private void btnShowBuy_Click(object sender, RoutedEventArgs e)
        {
            VisualState(EnumVisualState.ShowBuy);
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            bool isvalid = false;

            try
            {
                isvalid = Licensing.I.ValidateLicenseKey(txLicenseID.Text, "enduser@ifb");

                if (isvalid)
                {
                    // update machine id setting immediately
                    if (Properties.Settings.Default.MachineID == string.Empty)
                    {
                        Properties.Settings.Default.MachineID = FingerPrint.Value();
                    }
                    if (Properties.Settings.Default.LicenseID == string.Empty)
                    {
                        Properties.Settings.Default.LicenseID = txLicenseID.Text;
                    }

                    Properties.Settings.Default.Save();

                    Properties.Settings.Default.IsTrial = false;
                    this.TrialOver = false;
                    this.RenamedFiles = 0;
                    VisualState(EnumVisualState.Page1);

                    MessageBox.Show("Thank you for purchasing Image File Browser!");
                }
                else
                {
                    MessageBox.Show("Invalid License Key!");
                }
            }
            catch (WebException wex)
            {
                MessageBox.Show("Could not contact the licensing server... Please try again later");
            }
        }

        private void btnVisitProductSite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://wall.jaysonragasa.net/wall/post/2012/11/26/Image-File-Beautifier-Application.aspx");
        }

        #endregion

        #region // Methods /

        void StartScan(string source, string dest)
        {
            VisualState(EnumVisualState.Page2);
            
            byte alpha = 255;
            byte red = (byte)(r.Next(128) + 128);
            byte green = (byte)(r.Next(128) + 128);
            byte blue = (byte)(r.Next(128) + 128);

            bool isSourceAccessible = false;

            if (Directory.Exists(source))
            {
                isSourceAccessible = FileFolder.Instance.isAccessible(source);
                if (isSourceAccessible == false)
                {
                    MessageBox.Show("The selected folder cannot be accessed due to some security reasons. Please make sure that you have an access to the contents of the selected folder before continuing", "Security", MessageBoxButton.OK, MessageBoxImage.Information);

                    return;
                }
            }

            string output_destination = dest;
            if (File.Exists(dest))
            {
                output_destination = System.IO.Path.GetDirectoryName(dest);
            }
            
            this.vmme.EnqueueNewSource(new IFBMVVM.Model.UI.Model_WorkDetails()
            {
                WorkName = source,
                SourcePath = source,
                Destination = output_destination,
                ProgressBarMaximum = 0,
                ProgressBarValue = 0,
                ScanSubdirectories = cbScanSubdirectory.IsChecked.Value,
                ItemBackground = new SolidColorBrush(Color.FromArgb(alpha, red, green, blue)),
                UseDateModified = Properties.Settings.Default.UseDateModified

                //TodoWhenDuplicate = GetTodoByIndex(Properties.Settings.Default.TodoWhenDuplicate),
                //YearPrefix = Properties.Settings.Default.YearFolderPrefix,
                //MonthPrefix = Properties.Settings.Default.MonthFolderPrefix,
                //FileNameFormat = Properties.Settings.Default.CurrentFileNamingFormat
            });
        }

        void LoadSettings()
        {
            txYearFolderPrefix.Text = Properties.Settings.Default.YearFolderPrefix;
            txMonthFolderPrefix.Text = Properties.Settings.Default.MonthFolderPrefix;
            cbFileNameFormats.Items.Clear();
            cbFileNameFormats.ItemsSource = null;
            cbFileNameFormats.ItemsSource = Properties.Settings.Default.FileNamingFormats;
            cbFileNameFormats.SelectedItem = Properties.Settings.Default.CurrentFileNamingFormat;
            cbTodoWhenDups.SelectedIndex = Properties.Settings.Default.TodoWhenDuplicate;
            cbUseDateModified.IsChecked = Properties.Settings.Default.UseDateModified;
            txLimit.Text = Properties.Settings.Default.RenamedPictureCounter + " of 500 renamed";
            cbScanSubdirectory.IsChecked = Properties.Settings.Default.IncludeSubfolders;

            if (Properties.Settings.Default.RenamedPictureCounter == 500)
            {
                TrialIsOver();
            }

            bool isValid = false;

            //try
            //{
            //    if (Properties.Settings.Default.IsTrial == false)
            //    {
            //        isValid = Licensing.I.MachineCheck(Properties.Settings.Default.LicenseID, "enduser@ifb", Properties.Settings.Default.MachineID);

            //        if (isValid)
            //        {
            //        }
            //        else
            //        {
            //            MessageBox.Show("Using this product to other PC is prohibited. Please email jayson.d.ragasa@gmail.com regarding this problem");
            //        }
            //    }
            //}
            //catch (WebException wex)
            //{
                
            //}
        }

        void SaveSettings()
        {
            //if (Properties.Settings.Default.MachineID == string.Empty)
            //{
            //    Properties.Settings.Default.MachineID = FingerPrint.Value();
            //}

            Properties.Settings.Default.YearFolderPrefix = txYearFolderPrefix.Text;
            Properties.Settings.Default.MonthFolderPrefix = txMonthFolderPrefix.Text;
            Properties.Settings.Default.CurrentFileNamingFormat = cbFileNameFormats.SelectedItem.ToString();
            Properties.Settings.Default.TodoWhenDuplicate = cbTodoWhenDups.SelectedIndex;
            Properties.Settings.Default.UseDateModified = cbUseDateModified.IsChecked.Value;
            Properties.Settings.Default.IncludeSubfolders = cbScanSubdirectory.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private TODO_WHEN_DUPLICATE GetTodoByIndex(int index)
        {
            return (TODO_WHEN_DUPLICATE)Enum.Parse(typeof(TODO_WHEN_DUPLICATE), index.ToString());
        }

        void TrialIsOver()
        {
            if (Properties.Settings.Default.RenamedPictureCounter >= 500)
            {
                TrialOver = true;
                MessageBoxResult mbr = MessageBox.Show("You reached the maximum allowed image file beautification. If you want to use the full version, you can purchase a license key for just $10! Do you want to purchase now?", "Trial Version", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (mbr == MessageBoxResult.Yes)
                {
                    VisualState(EnumVisualState.ShowAbout);
                    VisualState(EnumVisualState.ShowBuy);
                }
            }
        }

        #endregion

        #region iMainWindow implementation
        public Image OpenButtonControl
        {
            get { return this.btnOpenFolder; }
            set { this.btnOpenFolder = value; }
        }
        public ListBox ListBoxControl
        {
            get { return this.listWork; }
            set { this.listWork = value; }
        }
        #endregion

        #region iStatus imple
        string _logtext = string.Empty;
        public string LogText
        {
            get { return this._logtext; }
            set
            {
                this._logtext = value;
                Status.Instance.LogText(value);
            }
        }

        int _renamedfiles = 0;
        public int RenamedFiles
        {
            get
            {
                return _renamedfiles;
            }
            set
            {
                this._renamedfiles = value;

                if (Properties.Settings.Default.IsTrial)
                {
                    Properties.Settings.Default.RenamedPictureCounter++;
                    txLimit.Text = Properties.Settings.Default.RenamedPictureCounter + " of 500 renamed";

                    TrialIsOver();
                }
            }
        }

        bool _trialover = false;
        public bool TrialOver
        {
            get
            {
                return _trialover;
            }
            set { _trialover = value; }
        }
        #endregion

        #region iNamingFormat imple
        public string YearPrefix
        {
            get { return Properties.Settings.Default.YearFolderPrefix; }
            set { Properties.Settings.Default.YearFolderPrefix = txYearFolderPrefix.Text; }
        }
        public string MonthPrefix
        {
            get { return Properties.Settings.Default.MonthFolderPrefix; }
            set { Properties.Settings.Default.MonthFolderPrefix = txMonthFolderPrefix.Text; }
        }
        public string FileNameFormat
        {
            get { return Properties.Settings.Default.CurrentFileNamingFormat; }
            set { Properties.Settings.Default.CurrentFileNamingFormat = cbFileNameFormats.SelectedItem.ToString(); }
        }
        public TODO_WHEN_DUPLICATE TodoWhenDuplicate
        {
            get { return GetTodoByIndex(Properties.Settings.Default.TodoWhenDuplicate); }
            set { Properties.Settings.Default.TodoWhenDuplicate = cbTodoWhenDups.SelectedIndex; }
        }
        //public bool UseDateModified
        //{
        //    get { return Properties.Settings.Default.UseDateModified; }
        //    set { Properties.Settings.Default.UseDateModified = cbUseDateModified.IsChecked.Value; }
        //}
        #endregion
    }
}
