using System;
using System.Collections.Generic;
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
    /// Interaktionslogik für VacWatch_nlUserControl.xaml
    /// </summary>
    public partial class VacWatch_nlUserControl : UserControl
    {
        public VacWatch_nlUserControl()
        {
            InitializeComponent();
        }

        private void UploadToVacWatchCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            VacWatch_nlSender.Send();
        }
    }
}
