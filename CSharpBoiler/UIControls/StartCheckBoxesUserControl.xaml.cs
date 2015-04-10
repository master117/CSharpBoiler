using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace CSharpBoiler.UIControls
{
    /// <summary>
    /// Interaktionslogik für StartCheckBoxesUserControlInstance.xaml
    /// </summary>
    public partial class StartCheckBoxesUserControl : UserControl
    {
        public const string AutoStarterName = "CSharpBoilerStarter.exe";

        public StartCheckBoxesUserControl()
        {
            InitializeComponent();

            if (IsStartupItem())
                AutoStartCheckBox.IsChecked = true;
        }

        #region AutoStart

        private void AutoStartCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (((CheckBox)sender).IsChecked == true)
            {
                if (!IsStartupItem())
                    // Add the value in the registry so that the application runs at startup
                    rkApp.SetValue(AutoStarterName, Directory.GetCurrentDirectory() + AutoStarterName);
            }
            else
            {
                if (IsStartupItem())
                    // Remove the value from the registry so that the application doesn't start
                    rkApp.DeleteValue(AutoStarterName, false);
            }
        }

        private bool IsStartupItem()
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rkApp.GetValue(AutoStarterName) == null)
                // The value doesn't exist, the application is not set to run at startup
                return false;
            else
                // The value exists, the application is set to run at startup
                return true;
        }
        #endregion
    }
}
