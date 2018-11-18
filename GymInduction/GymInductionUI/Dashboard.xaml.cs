using GymLibrary;
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

namespace GymInductionUI
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public User user = new User();
        public Dashboard()
        {
            InitializeComponent();
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            Admin admin = new Admin();
            frmMain.Navigate(admin);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnClient_Click(object sender, RoutedEventArgs e)
        {
            Client client = new Client();
            
            client.Show();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Search search = new Search();
            frmMain.Navigate(search);
        }

        private void checkUserAccess(User user)
        {
            if(user.LevelId== 1)
            {
                btnAdmin.Visibility = Visibility.Visible;    
            }
            if (user.LevelId == 2)
            {
                btnAdmin.Content = "Schedule";
                btnAdmin.Visibility = Visibility.Visible;
            }
            if (user.LevelId == 4)
            {
                btnClient.Visibility = Visibility.Hidden;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            checkUserAccess(user);
        }
    }
}
