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
        //connection string
        GymDbEntities db = new GymDbEntities("metadata=res://*/GymModel.csdl|res://*/GymModel.ssdl|res://*/GymModel.msl;provider = System.Data.SqlClient; provider connection string='data source = 192.168.1.110; initial catalog = GymDb; user id = GymUser; password=Pass.00*;pooling=False;MultipleActiveResultSets=True;App=EntityFramework'");

        //logging service
        LoggingProcess loggingProcess = new LoggingProcess();
        Log logtoSave = new Log();

        LoginProcess loginProcess = new LoginProcess();
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
            validCredentials = loginProcess.ValidateUserInput(currentUser, currentPassword);
            if (validCredentials)
            {
                validatedUser = getUserRecord(currentUser,currentPassword);
                if(validatedUser.UserId>0)
                {

                    logtoSave = loggingProcess.CreateLoginLogEntry("Login", "Success for", validatedUser.UserId, validatedUser.Username);
                    saveLogRecord(logtoSave);
                    Dashboard dashboard = new Dashboard();
                    dashboard.user = validatedUser;
                    dashboard.Owner = this;
                    dashboard.ShowDialog();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("The credentials you entered do not exist on the Database. Please check and try again.","User Login",MessageBoxButton.OK,MessageBoxImage.Error);
                    logtoSave = loggingProcess.CreateLoginLogEntry("Login", "Failure for", 0, "Credentials: "+currentUser+"/"+currentPassword);
                    saveLogRecord(logtoSave);
                }
                
            }
            else
            {
                MessageBox.Show("Invalid Username or Password. Please check and try again.", "User Login", MessageBoxButton.OK, MessageBoxImage.Error);
                logtoSave = loggingProcess.CreateLoginLogEntry("Login", "Invalid for", 0, "Credentials: " + currentUser + "/" + currentPassword);
                saveLogRecord(logtoSave);
            }
           
           
           

        }

        

        private int saveLogRecord(GymLibrary.Log log)
        {
            int saveSuccess = 0;
            try
            {
                db.Entry(log).State = System.Data.Entity.EntityState.Added;
                saveSuccess = db.SaveChanges();
            }
            catch (Exception)
            {
                MessageBox.Show("Error saving Login Log record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }
            return saveSuccess;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Environment.Exit(0);
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

        private User findUserByUserId(int userId)
        {
            User foundUser = new User();
            foreach (var user in db.Users)
            {
                if(user.UserId == userId)
                {
                    foundUser = user;
                }

            }
            return foundUser;
        }


    }
}
