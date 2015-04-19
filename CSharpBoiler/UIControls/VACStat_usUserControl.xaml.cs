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
        private bool wasChecked = false;
        private List<string> UserLists; 

        public VACStat_usUserControl()
        {
            InitializeComponent();

            if (Properties.Settings.Default.VACStat_usKey != "")
            {
                VacStat_usCheckBox.IsChecked = true;
                wasChecked = true;
                VAC_Stats_usAPIKEYTextBox.Text = Properties.Settings.Default.VACStat_usKey;
            }
        }

        public bool IsChecked()
        {
            return VacStat_usCheckBox.IsChecked == true;
        }

        private void VacStat_usCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (wasChecked && !IsChecked())
                Properties.Settings.Default.VACStat_usKey = "";

            if (IsChecked())
            {
                if (VAC_Stats_usAPIKEYTextBox.Text == "")
                {
                    MessageBox.Show("Please enter a Valid API KEY");
                    VacStat_usCheckBox.IsChecked = false;
                    return;
                }

                VACStat_usSender.GetLists(VAC_Stats_usAPIKEYTextBox.Text);
            }
        }
    }
}
