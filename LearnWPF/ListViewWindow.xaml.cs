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
using System.Windows.Shapes;

namespace LearnWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ListViewWindow : Window
    {
        private bool closedByApp = false;
        private List<User> items;

        public class User
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public string Mail { get; set; }
        }

        public ListViewWindow()
        {
            InitializeComponent();

            items = new List<User>();
            items.Add(new User() { Name = "John Doe", Age = 42, Mail = "john@doe-family.com" });
            items.Add(new User() { Name = "Jane Doe", Age = 39, Mail = "jane@doe-family.com" });
            items.Add(new User() { Name = "Sammy Doe", Age = 7, Mail = "sammy.doe@gmail.com" });
            lvUsers.ItemsSource = items;
        }

        public void CloseByApp()
        {
            closedByApp = true;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            if (!closedByApp)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            items.Clear();
            items.Add(new User() { Name = "Hello", Age = 42, Mail = "john@doe-family.com" });
            items.Add(new User() { Name = "Hollo2", Age = 39, Mail = "jane@doe-family.com" });
            items.Add(new User() { Name = "Bot.Name", Age = 7, Mail = "sammy.doe@gmail.com" });

            items[1].Mail = "bot@wz.com";

            lvUsers.Items.Refresh();
        }
    }
}
