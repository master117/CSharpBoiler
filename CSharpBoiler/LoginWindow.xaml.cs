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

            if (StartCheckBoxesUserControlInstance.IsAutoLoginEnabled())
            {
                UserNameTextBox.Text = StartCheckBoxesUserControlInstance.LoginDataModel.username;
                PasswordTextBox.Password = StartCheckBoxesUserControlInstance.LoginDataModel.password;

                Login();
            }
        }

        public void Login()
        {
            //SteamApp.Login(UserNameTextBox.Text, PasswordTextBox.Text);
            StartCheckBoxesUserControlInstance.LoginDataModel.username = UserNameTextBox.Text;
            StartCheckBoxesUserControlInstance.LoginDataModel.password = PasswordTextBox.Password;

            loginThread = new Thread(() => SteamApp.Login(StartCheckBoxesUserControlInstance.LoginDataModel.username, StartCheckBoxesUserControlInstance.LoginDataModel.password));
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

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                StartCheckBoxesUserControlInstance.IsAutoLoginEnabled();
                StartCheckBoxesUserControlInstance.StoreLogin(); 
                LoggedInEvent(SteamApp.steamUser.SteamID.AccountID);
            }));

            //LoggedInEvent(SteamApp.steamUser.SteamID.AccountID);            
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
