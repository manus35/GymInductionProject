using GymLibrary;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
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
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Page
    {

        GymDbEntities db = new GymDbEntities("metadata=res://*/GymModel.csdl|res://*/GymModel.ssdl|res://*/GymModel.msl;provider = System.Data.SqlClient; provider connection string='data source = 192.168.1.110; initial catalog = GymDb; user id = GymUser; password=Pass.00*;pooling=False;MultipleActiveResultSets=True;App=EntityFramework'");
        List<User> users = new List<User>();
        List<Log> logs = new List<Log>();
        List<Evaluation> evaluations = new List<Evaluation>();
        User selectedUser = new User();
        float avgHr = 0;
        double avgHeight = 0;
        double avgWeight = 0;
        float avgAge = 0;
        
        int pendInductions = 0;
        int avgCount = 0;

        enum DBOperation
        {
            Add,Modify,Delete
        }
        DBOperation dbOperation = new DBOperation();
        public Admin()
        {
            InitializeComponent();
        }

        private void submenuAddUser_Click(object sender, RoutedEventArgs e)
        {
            stkUserDetails.Visibility = Visibility.Visible;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dbOperation == DBOperation.Add)
            {


                User user = new User();
                user.FirstName = tbxFirstName.Text.Trim();
                user.LastName = tbxLastName.Text.Trim();
                user.Username = tbxUsername.Text.Trim();
                user.Password = tbxPassword.Text.Trim();
                user.LevelId = cmbAccessLevel.SelectedIndex;



                // MessageBox.Show(cmbAccessLevel.SelectedIndex.ToString());
                // stkUserDetails.Visibility = Visibility.Collapsed;
                int saveStatus = saveUser(user);

                if (saveStatus == 1)
                {
                    MessageBox.Show("User saved Successfully.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    refreshUserList();
                    clearUserDetails();
                    stkUserDetails.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Problem saving User record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            if (dbOperation == DBOperation.Modify)
            {
                foreach (var user in db.Users.Where(t => t.UserId == selectedUser.UserId))
                {
                    user.FirstName = tbxFirstName.Text.Trim();
                    user.LastName = tbxLastName.Text.Trim();
                    user.Password = tbxPassword.Text.Trim();
                    user.Username = tbxUsername.Text.Trim();
                    user.LevelId = cmbAccessLevel.SelectedIndex;
                }
                int saveSuccess = db.SaveChanges();
                if (saveSuccess == 1)
                {
                    MessageBox.Show("User Modified Successfully.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    refreshUserList();
                    clearUserDetails();
                    stkUserDetails.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Problem Modifying User record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
        }

        public int saveUser(User user)
        {
            db.Entry(user).State = System.Data.Entity.EntityState.Added;
            int saveStatus = db.SaveChanges();
            return saveStatus;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            refreshUserList();
            lstLogs.ItemsSource = logs;
            
            foreach (var log in db.Logs)
            {
                logs.Add(log);
            }
            foreach (var evaluation in db.Evaluations)
            {
                avgHr = avgHr + evaluation.HeartRate;
                avgCount++;
                avgHeight = avgHeight + evaluation.Height;
                avgWeight = avgWeight + evaluation.Weight;
                
            }
            int avgAsInt = Convert.ToInt32(avgHr/avgCount);
            txbAvgHR.Text ="The average Heart Rate of all inductees is: "+ avgAsInt.ToString();
        }

        private void refreshUserList()
        {
            lstUsers.ItemsSource = users;
            users.Clear();
            foreach (var user in db.Users)
            {
                users.Add(user);
            }
            lstUsers.Items.Refresh();
        }

        private void clearUserDetails()
        {
            tbxFirstName.Text = " ";
            tbxLastName.Text = " ";
            tbxPassword.Text = " ";
            tbxUsername.Text = " ";
            cmbAccessLevel.SelectedIndex=0;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
        
        }

        private void lstUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstUsers.SelectedIndex > 0)
            {
                selectedUser = users.ElementAt(lstUsers.SelectedIndex);
                submenuModUser.IsEnabled = true;
                submenuDelUser.IsEnabled = true;

                if (dbOperation == DBOperation.Add)
                {
                    clearUserDetails();
                }
                if (dbOperation == DBOperation.Modify)
                {
                    tbxFirstName.Text = selectedUser.FirstName;
                    tbxLastName.Text = selectedUser.LastName;
                    tbxUsername.Text = selectedUser.Username;
                    tbxPassword.Text = selectedUser.Password;
                    cmbAccessLevel.SelectedIndex = selectedUser.LevelId;
                }
    
            }

        }

        private void submenuModUser_Click(object sender, RoutedEventArgs e)
        {
            dbOperation = DBOperation.Modify;
            //set fields to selected user
            selectedUser = users.ElementAt(lstUsers.SelectedIndex);
            tbxFirstName.Text = selectedUser.FirstName;
            tbxLastName.Text = selectedUser.LastName;
            tbxUsername.Text = selectedUser.Username;
            tbxPassword.Text = selectedUser.Password;
            cmbAccessLevel.SelectedIndex = selectedUser.LevelId;
            stkUserDetails.Visibility = Visibility.Visible;
        }

        private void submenuDelUser_Click(object sender, RoutedEventArgs e)
        {
            db.Users.RemoveRange(db.Users.Where(t => t.UserId == selectedUser.UserId));
            int saveSuccess = db.SaveChanges();
            if (saveSuccess == 1)
            {
                MessageBox.Show("User Deleted Successfully.", "Delete from Database", MessageBoxButton.OK, MessageBoxImage.Information);
                refreshUserList();
                clearUserDetails();
                stkUserDetails.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Problem Deleting User record.", "Delete Database", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
            
        }
    }
}
