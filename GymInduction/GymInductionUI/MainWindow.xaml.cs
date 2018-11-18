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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GymInductionUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GymDbEntities db = new GymDbEntities("metadata=res://*/GymModel.csdl|res://*/GymModel.ssdl|res://*/GymModel.msl;provider = System.Data.SqlClient; provider connection string='data source = 192.168.1.110; initial catalog = GymDb; user id = GymUser; password=Pass.00*;pooling=False;MultipleActiveResultSets=True;App=EntityFramework'");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            String currentUser = tbxUsername.Text;
            String currentPassword = tbxPassword.Password;
            foreach (var user in db.Users)
            {
                if (user.Username == currentUser && user.Password == currentPassword)
                {
                    Dashboard dashboard = new Dashboard();
                    dashboard.user = user;
                    dashboard.ShowDialog();
                    this.Hide();
                }
                else
                {
                    lblError.Content = "Please Check Username and Password";
                }
                
            }

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
