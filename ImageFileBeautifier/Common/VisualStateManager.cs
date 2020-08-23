using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageFileBeautifier
{
    public partial class MainWindow
    {
        public enum EnumVisualState
        {
            Page1,
            Page2,

            Beautifying,
            BeautifyingDone,

            DragFolder,
            DragNotFolder,

            ShowAbout,
            HideAbout,

            ShowSettings,
            HideSettings,

            ShowAboutGrid,
            ShowPrivacyGrid,
            ShowHelpGrid,
            ShowBuy
        }

        public void VisualState(EnumVisualState state)
        {
            switch (state)
            {
                case EnumVisualState.Page1:
                    gridMainPage.Height = 360;
                    gridMainPage.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    gridWorkPage.Visibility = System.Windows.Visibility.Collapsed;
                    btnOpenFolder.Height = 128;
                    btnOpenFolder.Width = 128;
                    btnOpenFolder_hover.Width = 128;
                    btnOpenFolder_hover.Height = 128;

                    gridAbout.Visibility = System.Windows.Visibility.Collapsed;
                    gridSettings.Visibility = System.Windows.Visibility.Collapsed;

                    lblTrialVersion.Visibility = Properties.Settings.Default.IsTrial ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                    txLimit.Visibility = Properties.Settings.Default.IsTrial ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

                    break;

                case EnumVisualState.Page2:
                    gridMainPage.Height = 110;
                    gridMainPage.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    gridWorkPage.Visibility = System.Windows.Visibility.Visible;
                    btnOpenFolder.Height = 64;
                    btnOpenFolder.Width = 64;
                    btnOpenFolder_hover.Width = 64;
                    btnOpenFolder_hover.Height = 64;
                    break;

                case EnumVisualState.Beautifying:
                    panelOpenFolderIcon.Visibility = System.Windows.Visibility.Collapsed;
                    panelBStarted.Visibility = System.Windows.Visibility.Visible;
                    break;

                case EnumVisualState.BeautifyingDone:
                    panelOpenFolderIcon.Visibility = System.Windows.Visibility.Visible;
                    panelBStarted.Visibility = System.Windows.Visibility.Collapsed;
                    btnOpenFolder_hover.Visibility = System.Windows.Visibility.Collapsed;
                    btnOpenFolder.Visibility = System.Windows.Visibility.Visible;
                    break;

                case EnumVisualState.DragFolder:
                    btnOpenFolder_hover.Visibility = System.Windows.Visibility.Visible;
                    btnOpenFolder.Visibility = System.Windows.Visibility.Collapsed;
                    break;

                case EnumVisualState.DragNotFolder:
                    btnOpenFolder_hover.Visibility = System.Windows.Visibility.Collapsed;
                    btnOpenFolder.Visibility = System.Windows.Visibility.Visible;
                    break;

                case EnumVisualState.ShowAbout:
                    gridAbout.Visibility = System.Windows.Visibility.Visible;
                    gridShowAbout.Visibility = System.Windows.Visibility.Visible;
                    gridShowPrivacyStatement.Visibility = System.Windows.Visibility.Collapsed;
                    gridShowHelp.Visibility = System.Windows.Visibility.Collapsed;
                    gridShowBuy.Visibility = System.Windows.Visibility.Collapsed;
                    gridSettings.Visibility = System.Windows.Visibility.Collapsed;

                    if (Properties.Settings.Default.IsTrial == false)
                    {
                        btnShowBuy.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    break;

                case EnumVisualState.HideAbout:
                    gridAbout.Visibility = System.Windows.Visibility.Collapsed;
                    break;

                case EnumVisualState.ShowSettings:
                    gridSettings.Visibility = System.Windows.Visibility.Visible;
                    gridAbout.Visibility = System.Windows.Visibility.Collapsed;
                    break;

                case EnumVisualState.HideSettings:
                    gridSettings.Visibility = System.Windows.Visibility.Collapsed;
                    break;

                case EnumVisualState.ShowAboutGrid:
                    gridShowAbout.Visibility = System.Windows.Visibility.Visible;
                    gridShowPrivacyStatement.Visibility = System.Windows.Visibility.Collapsed;
                    gridShowHelp.Visibility = System.Windows.Visibility.Collapsed;
                    gridShowBuy.Visibility = System.Windows.Visibility.Collapsed;
                    break;

                case EnumVisualState.ShowHelpGrid:
                    gridShowAbout.Visibility = System.Windows.Visibility.Collapsed;
                    gridShowPrivacyStatement.Visibility = System.Windows.Visibility.Collapsed;
                    gridShowHelp.Visibility = System.Windows.Visibility.Visible;
                    gridShowBuy.Visibility = System.Windows.Visibility.Collapsed;
                    break;

                case EnumVisualState.ShowPrivacyGrid:
                    gridShowAbout.Visibility = System.Windows.Visibility.Collapsed;
                    gridShowPrivacyStatement.Visibility = System.Windows.Visibility.Visible;
                    gridShowHelp.Visibility = System.Windows.Visibility.Collapsed;
                    gridShowBuy.Visibility = System.Windows.Visibility.Collapsed;
                    break;

                case EnumVisualState.ShowBuy:
                    gridShowAbout.Visibility = System.Windows.Visibility.Collapsed;
                    gridShowPrivacyStatement.Visibility = System.Windows.Visibility.Collapsed;
                    gridShowHelp.Visibility = System.Windows.Visibility.Collapsed;
                    gridShowBuy.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
        }
    }
}
