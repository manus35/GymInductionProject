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
            User validatedUser = new User();
            bool login = false;
            String currentUser = tbxUsername.Text;
            String currentPassword = tbxPassword.Password;
            foreach (var user in db.Users)
            {
                if (user.Username == currentUser && user.Password == currentPassword)
                {
                    login = true;
                    validatedUser = user;
                }
                else
                {
                    lblError.Content = "Please Check Username and Password";
                }
                
            }
            if (login)
            {
                CreateLogEntry("Login", "User loged In",validatedUser.UserId,validatedUser.Username);
                Dashboard dashboard = new Dashboard();
                dashboard.user = validatedUser;
                dashboard.Owner= this;
                dashboard.ShowDialog();
                this.Hide();
            }
            else
            {
                CreateLogEntry("Login", "User login Unsuccessful", 0, validatedUser.Username);
            }

        }

        private void CreateLogEntry(String category, String description, int userId, String username)
        {
            string comment = $"{description} user credentials  = {username}";
            Log log = new Log();
            log.UserId = userId;
            log.Category = category;
            log.Description = comment;
            log.Date = DateTime.Now;
            saveLog(log);
        }

        private void saveLog(Log log)
        {
            db.Entry(log).State = System.Data.Entity.EntityState.Added;
            db.SaveChanges();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Environment.Exit(0);
        }
    }
}
