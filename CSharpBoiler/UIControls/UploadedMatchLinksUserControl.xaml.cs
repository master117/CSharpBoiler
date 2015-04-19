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

namespace CSharpBoiler.UIControls
{
    /// <summary>
    /// Interaktionslogik für UploadedMatchLinksUserControl.xaml
    /// </summary>
    public partial class UploadedMatchLinksUserControl : UserControl
    {
        private const string TextStart = "Uploaded MatchLinks: ";

        public UploadedMatchLinksUserControl()
        {
            InitializeComponent();
        }

        public void SetUploadedMatchLinksCount(int uploaded, int max)
        {
            UploadedMatchLinksTextBlock.Text = TextStart + uploaded + "/" + max;
        }
    }
}
