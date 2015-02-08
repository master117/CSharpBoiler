using System;
using System.Collections.Generic;
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
        private long steamID;

        public LoginWindow()
        {
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SteamApp.SteamLoggedOnCallback = LoggedOnEvent;
            SteamApp.Need2FactorCallback = TwoFactorEvent;
            SteamApp.NeedAuthCodeCallback = AuthCodeEvent;
        }

        public void Login()
        {
            //SteamApp.Login(UserNameTextBox.Text, PasswordTextBox.Text);

            string username = UserNameTextBox.Text;
            string password = PasswordTextBox.Text;

            loginThread = new Thread(() => SteamApp.Login(username, password));
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
            Application.Current.Dispatcher.Invoke(new Action(() => { LoggedInEvent(SteamApp.steamUser.SteamID.AccountID); }));

            //LoggedInEvent(SteamApp.steamUser.SteamID.AccountID);            
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }
    }
}
