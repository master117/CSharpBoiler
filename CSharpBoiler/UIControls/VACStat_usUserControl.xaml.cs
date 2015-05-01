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

            if (Properties.Settings.Default.VACStat_usKey != "")
            {
                VacStat_usCheckBox.IsChecked = true;
                ApikeyDockPanel.Visibility = Visibility.Collapsed;
                ListsDockPanel.Visibility = Visibility.Visible;
            }
        }

        public bool IsChecked()
        {
            return VacStat_usCheckBox.IsChecked == true;
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
        }

        private void GetListsButton_OnClick(object sender, RoutedEventArgs e)
        {
            VAC_Stats_usListComboBox.ItemsSource = VACStat_usSender.GetLists(Properties.Settings.Default.VACStat_usKey);
            VAC_Stats_usListComboBox.SelectedIndex = 0;

            ApikeyDockPanel.Visibility = Visibility.Collapsed;
            ListsDockPanel.Visibility = Visibility.Visible;
        }

        private void ChooseListButton_OnClick(object sender, RoutedEventArgs e)
        {
            int selectedIndex = VAC_Stats_usListComboBox.SelectedIndex;
            Dictionary<int, string> listDictionary = (Dictionary<int, string>) VAC_Stats_usListComboBox.ItemsSource;

            if (selectedIndex >= 0 && selectedIndex < listDictionary.Count)
            {
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
