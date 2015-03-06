using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;
using System.Xml.Serialization;

namespace CSharpBoiler
{
    /// <summary>
    /// Interaktionslogik für LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        Thread loginThread;
        public delegate void LoggedIn(long steamID);
        public static event LoggedIn LoggedInEvent;
        public bool checkBoxChecked;

        private LoginDataModel mainLoginDataModel = new LoginDataModel();


        public LoginWindow()
        {
            InitializeComponent();

            MouseDown += Window_MouseDown;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SteamApp.SteamLoggedOnCallback = LoggedOnEvent;
            SteamApp.Need2FactorCallback = TwoFactorEvent;
            SteamApp.NeedAuthCodeCallback = AuthCodeEvent;

            DeserializeLoginData();

            if (mainLoginDataModel != null && mainLoginDataModel.username != null && mainLoginDataModel.password != null)
            {
                UserNameTextBox.Text = mainLoginDataModel.username;
                PasswordTextBox.Password = mainLoginDataModel.password;

                AutoLoginCheckBox.IsChecked = true;
                Login();
            }
        }

        public void Login()
        {
            //SteamApp.Login(UserNameTextBox.Text, PasswordTextBox.Text);

            mainLoginDataModel.username = UserNameTextBox.Text;
            mainLoginDataModel.password = PasswordTextBox.Password;

            loginThread = new Thread(() => SteamApp.Login(mainLoginDataModel.username, mainLoginDataModel.password));
            loginThread.SetApartmentState(ApartmentState.STA);
            loginThread.Start();
        }

        void TwoFactorEvent()
        {
            var dialog = new AuthMessageBox();
            if (dialog.ShowDialog() == true)
            {
                SteamApp.twoFactorAuth = dialog.ResponseText;
            }
            //loginThread.Suspend();        
        }

        void AuthCodeEvent()
        {
            var dialog = new AuthMessageBox();
            if (dialog.ShowDialog() == true)
            {
                SteamApp.authCode = dialog.ResponseText;
            }
            //loginThread.Suspend();        
        }

        void LoggedOnEvent()
        {
            //loginThread.Suspend();
            if (checkBoxChecked)
            {
                SerializeLoginData();  
            }
            else
            {
                if (File.Exists("CSharpBoilerLogin"))
                    File.Delete("CSharpBoilerLogin");   
            }

            Application.Current.Dispatcher.Invoke(new Action(() => { LoggedInEvent(SteamApp.steamUser.SteamID.AccountID); }));

            //LoggedInEvent(SteamApp.steamUser.SteamID.AccountID);            
        }

        private void SerializeLoginData()
        {
            //Retrieving old matches
            if (File.Exists("CSharpBoilerLogin.xml"))
                File.Delete("CSharpBoilerLogin.xml");

            XmlSerializer xmlWriter = new XmlSerializer(typeof(LoginDataModel));

            StreamWriter loginDataStreamWriter = new StreamWriter("CSharpBoilerLogin.xml");
            xmlWriter.Serialize(loginDataStreamWriter, mainLoginDataModel);
            loginDataStreamWriter.Close();
        }

        private void DeserializeLoginData()
        {
            if (File.Exists("CSharpBoilerLogin.xml"))
            {
                System.Xml.Serialization.XmlSerializer xmlReader =
                    new System.Xml.Serialization.XmlSerializer(typeof(LoginDataModel));
                System.IO.StreamReader loginDataStreamReader = new System.IO.StreamReader(
                    "CSharpBoilerLogin.xml");
                LoginDataModel tempLoginDataModel = (LoginDataModel)xmlReader.Deserialize(loginDataStreamReader);

                loginDataStreamReader.Close();

                mainLoginDataModel = tempLoginDataModel;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AutoLoginCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            checkBoxChecked = (AutoLoginCheckBox.IsChecked == true);
        }
    }
}
