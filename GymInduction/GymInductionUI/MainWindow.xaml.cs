using GymLibrary;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
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
            bool validCredentials = false;
            String currentUser = tbxUsername.Text;
            String currentPassword = tbxPassword.Password;
            validCredentials = ValidateUserInput(currentUser, currentPassword);
            if (validCredentials)
            {
                validatedUser = getUserRecord(currentUser,currentPassword);
                if(validatedUser.UserId>0)
                {

                    CreateLogEntry("Login", "User loged In", validatedUser.UserId, validatedUser.Username);
                    Dashboard dashboard = new Dashboard();
                    dashboard.user = validatedUser;
                    dashboard.Owner = this;
                    dashboard.ShowDialog();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("The credentials you entered do not exist on the Database. Please check and try again.","User Login",MessageBoxButton.OK,MessageBoxImage.Error);
                    CreateLogEntry("Login", "User login Unsuccessful", 0, "Credentials: "+currentUser+"/"+currentPassword);
                }
                
            }
            else
            {
                MessageBox.Show("Invalid Username or Password. Please check and try again.", "User Login", MessageBoxButton.OK, MessageBoxImage.Error);
                CreateLogEntry("Login", "User login Unsuccessful", 0, "Credentials: " + currentUser + "/" + currentPassword);
            }
           
           
           

        }

        private void CreateLogEntry(String category, String description, int userId, String username)
        {
            string comment = $"{description} user credentials  = {username}";
            Log log = new Log();
            if (userId > 0)
            { 
            log.UserId = userId;
            }
            log.Category = category;
            log.Description = comment;
            log.Date = DateTime.Now;
            saveLog(log);
            if (userId > 0)
            { 
            log.UserId = userId;
            }
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
        /// <summary>
        /// Validates the user credentials against those in the database
        /// </summary>
        /// <param name="username">
        /// username entered by user
        /// </param>
        /// <param name="password">
        /// password entered by user
        /// </param>
        /// <returns>
        /// validated user
        /// </returns>
        public bool ValidateUserInput(string username, string password)
        {
            //initialise validated to true and pnly change to false when conditions are met
            bool validated = true;

            //check if username exists and is less than allowed 30 characters
            if (username.Length == 0 || username.Length > 30)
            {
                validated = false;
            }
            //check for numbers in username I will allow for now
            /*
            foreach(char ch in username )
            {
                if(ch >= '0' && ch <= '9')
                {
                    validated = false;
                }
            }
            */
            //password exists and is no more than the allowed 30 characters
            if (password.Length == 0 || password.Length > 30)
            {
                validated = false;
            }
            return validated;

        }
        /// <summary>
        /// validated user credentials against those in the sql db
        /// </summary>
        /// <param name="username">
        /// username entered by user
        /// </param>
        /// <param name="password">
        /// password entered by user
        /// </param>
        /// <returns>
        /// Validated User
        /// </returns>
        private User getUserRecord(string username, string password)
        {
            User validatedUser = new User();
            try
            {
                //gets username and password passed to the method from usertable in db
                
                foreach (var user in db.Users.Where(t => t.Username == username && t.Password == password))
                {
                    validatedUser = user;
                }
            }
            catch (EntityException ex)
            {
                MessageBox.Show("Problem connecting to SQL server. Application will now close. See Exception","Connect to Database"+ex.InnerException, MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                Environment.Exit(0);
                
            }
           
            return validatedUser;

        }
    }
}
