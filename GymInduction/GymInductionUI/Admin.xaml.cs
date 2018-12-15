﻿using GymLibrary;
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
        List<GymLibrary.Client> clients = new List<GymLibrary.Client>();
        List<Evaluation> evaluations = new List<Evaluation>();
        User selectedUser = new User();
        User currentLoggedOnUser = new User();
        float avgHr = 0;
        double avgHeight = 0;
        double avgWeight = 0;
        float avgAge = 0;

        int pendInductions = 0;
        int avgCount = 0;

        enum DBOperation
        {
            Add, Modify, Delete
        }

        enum AnalysisType
        {
            Summary, Count, Statistics
        }

        private AnalysisType analysisType = new AnalysisType();

        enum TableSelected
        {
            User, Client, Log
        }

        private TableSelected tableSelected = new TableSelected();

        DBOperation dbOperation = new DBOperation();
        public Admin(User user)
        {
            currentLoggedOnUser = user;
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
                    //CreateLogEntry("Database", "User Saved Successfully", currentLoggedOnUser.UserId, currentLoggedOnUser.Username);
                    refreshUserList();
                    clearUserDetails();
                    stkUserDetails.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Problem saving User record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Warning);
                    //CreateLogEntry("Database", "Problem Saving User Record", currentLoggedOnUser.UserId, currentLoggedOnUser.Username);
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
                    //CreateLogEntry("Database", "User Modified Successfully", currentLoggedOnUser.UserId, currentLoggedOnUser.Username);
                    refreshUserList();
                    clearUserDetails();
                    stkUserDetails.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Problem Modifying User record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Warning);
                   // CreateLogEntry("Database", "Problem modifying User", currentLoggedOnUser.UserId, currentLoggedOnUser.Username);
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

            foreach (var client in db.Clients)
            {
                clients.Add(client);
            }
            foreach (var evaluation in db.Evaluations)
            {
                avgHr = avgHr + evaluation.HeartRate;
                avgCount++;
                avgHeight = avgHeight + evaluation.Height;
                avgWeight = avgWeight + evaluation.Weight;

            }
            //int avgAsInt = Convert.ToInt32(avgHr/avgCount);
            //txbAvgHR.Text ="The average Heart Rate of all inductees is: "+ avgAsInt.ToString();
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
            cmbAccessLevel.SelectedIndex = 0;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            stkUserDetails.Visibility = Visibility.Collapsed;
            clearUserDetails();
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
                CreateLogEntry("Database", "Successful User Delete", currentLoggedOnUser.UserId, currentLoggedOnUser.Username);
                refreshUserList();
                clearUserDetails();
                stkUserDetails.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Problem Deleting User record.", "Delete Database", MessageBoxButton.OK, MessageBoxImage.Warning);
               //CreateLogEntry("Database", "Problem Deleting User", currentLoggedOnUser.UserId, currentLoggedOnUser.Username);
            }


        }

        private void cboAnalysis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //check which option is selected greater than 0 are valid
            if (cboAnalysis.SelectedIndex > 0)
            {
                if (cboAnalysis.SelectedIndex == 1)
                {
                    analysisType = AnalysisType.Summary;
                }
                if (cboAnalysis.SelectedIndex == 2)
                {
                    analysisType = AnalysisType.Count;
                }
                if (cboAnalysis.SelectedIndex == 3)
                {
                    analysisType = AnalysisType.Statistics;
                }
            }

        }

        private void cboChoose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //check which option is selected greater than 0 are valid
            if (cboChoose.SelectedIndex > 0)
            {
                if (cboChoose.SelectedIndex == 1)
                {
                    tableSelected = TableSelected.Client;
                }
                if (cboChoose.SelectedIndex == 2)
                {
                    tableSelected = TableSelected.User;
                }
                if (cboChoose.SelectedIndex == 3)
                {
                    tableSelected = TableSelected.Log;
                }
            }
        }

        private void btnAnalyse_Click(object sender, RoutedEventArgs e)
        {
            //Clear all vars before use
            int recordCount = 0;
            string output = "";
            tbkAnalysis.Text = "";
            if (analysisType == AnalysisType.Summary && tableSelected == TableSelected.Client)
            {
                foreach (var client in clients)
                {
                    recordCount++;
                    output = output + Environment.NewLine + $"Record {recordCount} is for Client" +
                        $" named {client.FirstName} {client.LastName}  " + Environment.NewLine;
                }
                output = output + Environment.NewLine + $"Total Client  Records in the Database  is {recordCount}" + Environment.NewLine;
                tbkAnalysis.Text = output;
            }

            if (analysisType == AnalysisType.Summary && tableSelected == TableSelected.User)
            {
                int level1Count = 0;
                int level2Count = 0;
                int level3Count = 0;
                int level4Count = 0;
                foreach (var user in users)
                {
                    recordCount++;
                    output = output + Environment.NewLine + $"Record {recordCount} is for User " +
                        $"whos name is {user.FirstName} {user.LastName} " +
                        $" with Username {user.Username} and Access Level {user.LevelId}, User role" +
                        $" is {user.AccessLevel.UserType}  " + Environment.NewLine;

                    if (user.LevelId == 1)
                    {
                        level1Count++;
                    }
                    if (user.LevelId == 2)
                    {
                        level2Count++;
                    }
                    if (user.LevelId == 3)
                    {
                        level3Count++;
                    }
                    if (user.LevelId == 4)
                    {
                        level4Count++;
                    }
                }

                output = output + Environment.NewLine + $"Total Users  with Level 1 Access is {level1Count}." + Environment.NewLine;
                output = output + Environment.NewLine + $"Total Users  with Level 2 Access is {level2Count}." + Environment.NewLine;
                output = output + Environment.NewLine + $"Total Users  with Level 3 Access is {level3Count}." + Environment.NewLine;
                output = output + Environment.NewLine + $"Total Users  with Level 4 Access is {level4Count}." + Environment.NewLine;
                output = output + Environment.NewLine + $"Total Users  in the Database is  {recordCount}." + Environment.NewLine;
                tbkAnalysis.Text = output;
            }

            if (analysisType == AnalysisType.Summary && tableSelected == TableSelected.Log)
            {
                foreach (var log in logs)
                {
                    recordCount++;
                    output = output + Environment.NewLine + $"Record {recordCount} is for Log created by {log.User.FirstName} {log.User.LastName} " +
                        $"whose UserId is {log.UserId}, " + Environment.NewLine +
                        $"log was created on  {log.Date} " + Environment.NewLine +
                        $" Log is registered for the category of {log.Category}" +
                        $" with description of {log.Description} " + Environment.NewLine;

                }
                output = output + Environment.NewLine + $"Total Logs  in the Database is  {recordCount}." + Environment.NewLine;
                tbkAnalysis.Text = output;
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
            /*
            if (userId > 0)
            { 
            log.UserId = userId;
            }
            log.Category = category;
            log.Description = comment;
            log.Date = DateTime.Now;
            saveLog(log);
            */
        }

        private void saveLog(Log log)
        {
            db.Entry(log).State = System.Data.Entity.EntityState.Added;

            db.SaveChanges();

        }
    }
}
