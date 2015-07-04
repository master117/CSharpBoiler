using System;
using System.Collections.Generic;
using System.Configuration;
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
using CSharpBoiler.NetworkHelper;

namespace CSharpBoiler.UIControls
{
    /// <summary>
    /// Interaktionslogik für VACStat_usUserControl.xaml
    /// </summary>
    public partial class VACStat_usUserControl : UserControl
    {
        public VACStat_usUserControl()
        {
            InitializeComponent();
        }

        private void VacStat_usCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            ApikeyDockPanel.Visibility = Visibility.Visible;
            ListsDockPanel.Visibility = Visibility.Collapsed;
        }

        private void VacStat_usCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ApikeyDockPanel.Visibility = Visibility.Collapsed;
            ListsDockPanel.Visibility = Visibility.Collapsed;
            Properties.Settings.Default.VACStat_usKey = "";
            Properties.Settings.Default.VACStat_usListId = 0;
        }

        private void GetListsButton_OnClick(object sender, RoutedEventArgs e)
        {
            VAC_Stats_usListComboBox.ItemsSource = VACStat_usSender.GetListsAsDictionary(Properties.Settings.Default.VACStat_usKey);
            VAC_Stats_usListComboBox.SelectedIndex = 0;

            if (VAC_Stats_usListComboBox.Items.Count > 0)
            {
                ApikeyDockPanel.Visibility = Visibility.Collapsed;
                ListsDockPanel.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show(
                    "There seems to be a problem obtaining the lists of this User or the APIKEY is wrong. " +
                    "Make sure that you have created at least one(1) list on VacStat.us and the APIKEY is correct.");
            }
        }

        private void ChooseListButton_OnClick(object sender, RoutedEventArgs e)
        {
            //APIKEY was entered and List was selected, upload all steamIds to this List.
            int selectedIndex = VAC_Stats_usListComboBox.SelectedIndex;         

            if (selectedIndex >= 0)
            {
                Dictionary<int, string> listDictionary = (Dictionary<int, string>)VAC_Stats_usListComboBox.ItemsSource;

                ListsDockPanel.Visibility = Visibility.Collapsed;

                Properties.Settings.Default.VACStat_usListId = listDictionary.ElementAt(selectedIndex).Key;
                VACStat_usSender.Send(Properties.Settings.Default.VACStat_usKey, listDictionary.ElementAt(selectedIndex).Key);
            }
        }

        private void ExplanationButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You can now automatically add users from this program to your VacStatus list! " +
                            "Inorder to allow this program to add users for you, " +
                            "go on profile settings page on vacstatus (https://vacstat.us/settings) " +
                            "and paste the private key to the textbox on this program. " +
                            "And after you have done that, select the list that you want the users inserted.");
        }
    }
}
