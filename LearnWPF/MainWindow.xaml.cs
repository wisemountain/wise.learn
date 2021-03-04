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

namespace LearnWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ListViewWindow lstWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if ( lstWindow != null )
            {
                lstWindow.CloseByApp();
                lstWindow.Close();
            }

            base.OnClosing(e);
        }

        private void MenuItem_Click_OpenListView(object sender, RoutedEventArgs e)
        {
            if (lstWindow == null)
            {
                lstWindow = new ListViewWindow();
            }

            lstWindow.Show();
        }
    }
}
