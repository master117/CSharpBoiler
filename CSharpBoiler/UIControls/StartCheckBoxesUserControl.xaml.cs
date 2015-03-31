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
        private LoginDataModel _LoginDataModel { get; set; }
        public LoginDataModel LoginDataModel 
        {
            get
            {
               return _LoginDataModel; 
            }
            private set
            {
               _LoginDataModel = value; 
            }
        }

        public const string AutoStarterName = "CSharpBoilerStarter.exe";

        public StartCheckBoxesUserControl()
        {
            InitializeComponent();

            DeserializeLoginData();

            if(LoginDataModel == null)
                LoginDataModel = new LoginDataModel();

            if (LoginDataModel.username != null)
                AutoLoginCheckBox.IsChecked = true;

            if (IsStartupItem())
                AutoStartCheckBox.IsChecked = true;
        }

        #region Login

        private void AutoLoginCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (AutoLoginCheckBox.IsChecked != true)
            {
                //Deleting old Login Data
                if (File.Exists("CSharpBoilerLogin.xml"))
                    File.Delete("CSharpBoilerLogin.xml");
            }
        }

        public void StoreLogin()
        {
                SerializeLoginData();
        }

        private void SerializeLoginData()
        {
            try
            {
                //Deleting old Login Data
                if (File.Exists("CSharpBoilerLogin.xml"))
                    File.Delete("CSharpBoilerLogin.xml");

                XmlSerializer xmlWriter = new XmlSerializer(typeof(LoginDataModel));

                StreamWriter loginDataStreamWriter = new StreamWriter("CSharpBoilerLogin.xml");
                xmlWriter.Serialize(loginDataStreamWriter, _LoginDataModel);
                loginDataStreamWriter.Close();
            }
            catch (Exception)
            {               
                throw;
            }

        }

        private void DeserializeLoginData()
        {
            if (File.Exists("CSharpBoilerLogin.xml"))
            {
                try
                {
                    System.Xml.Serialization.XmlSerializer xmlReader =
                        new System.Xml.Serialization.XmlSerializer(typeof (LoginDataModel));
                    System.IO.StreamReader loginDataStreamReader = new System.IO.StreamReader(
                        "CSharpBoilerLogin.xml");
                    LoginDataModel tempLoginDataModel = (LoginDataModel) xmlReader.Deserialize(loginDataStreamReader);

                    loginDataStreamReader.Close();

                    _LoginDataModel = tempLoginDataModel;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public bool IsAutoLoginEnabled()
        {
            return AutoLoginCheckBox.IsChecked == true;
        }
        #endregion

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
