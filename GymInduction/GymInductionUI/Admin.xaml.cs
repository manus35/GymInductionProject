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
        //connection string
        GymDbEntities db = new GymDbEntities("metadata=res://*/GymModel.csdl|res://*/GymModel.ssdl|res://*/GymModel.msl;provider = System.Data.SqlClient; provider connection string='data source = 192.168.1.110; initial catalog = GymDb; user id = GymUser; password=Pass.00*;pooling=False;MultipleActiveResultSets=True;App=EntityFramework'");

        //logging service
        LoggingProcess loggingProcess = new LoggingProcess();
        Log logtoSave = new Log();

        //instance variables
        List<User> users = new List<User>();
        List<Log> logs = new List<Log>();
        List<GymLibrary.Client> clients = new List<GymLibrary.Client>();
        List<Evaluation> evaluations = new List<Evaluation>();
        User selectedUser = new User();
        Instructor instructor = new Instructor();
        User currentLoggedOnUser = new User();
        float avgHr = 0;
        double avgHeight = 0;
        double avgWeight = 0;
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

                if(user.LevelId == 3)
                {
                    instructor.FName = user.FirstName;
                    instructor.LName = user.LastName;                   
                }
                int insSaveSuccess = saveInstructorRecord(instructor);
                int saveStatus = saveUser(user);

                if (saveStatus == 1)
                {
                    MessageBox.Show("User saved Successfully.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    logtoSave = loggingProcess.CreateAdminLogEntry("Database", "Successfully saved", currentLoggedOnUser.UserId, currentLoggedOnUser.Username,user);
                    saveLogRecord(logtoSave);
                    refreshUserList();
                    clearUserDetails();
                    stkUserDetails.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Problem saving User record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Warning);
                    logtoSave = loggingProcess.CreateAdminLogEntry("Database", "saving problem", currentLoggedOnUser.UserId, currentLoggedOnUser.Username,user);
                    saveLogRecord(logtoSave);
                }
            }
            if (dbOperation == DBOperation.Modify)
            {
                User userMod = new User();
                foreach (var user in db.Users.Where(t => t.UserId == selectedUser.UserId))
                {
                    user.FirstName = tbxFirstName.Text.Trim();
                    user.LastName = tbxLastName.Text.Trim();
                    user.Password = tbxPassword.Text.Trim();
                    user.Username = tbxUsername.Text.Trim();
                    user.LevelId = cmbAccessLevel.SelectedIndex;
                    userMod = user;
                }
                int saveSuccess = db.SaveChanges();
                if (saveSuccess == 1)
                {
                    MessageBox.Show("User Modified Successfully.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    logtoSave = loggingProcess.CreateAdminLogEntry("Database", "successfully modified", currentLoggedOnUser.UserId, currentLoggedOnUser.Username, userMod);
                    saveLogRecord(logtoSave);
                    refreshUserList();
                    clearUserDetails();
                    stkUserDetails.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Problem Modifying User record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Warning);
                    logtoSave = loggingProcess.CreateAdminLogEntry("Database", "Modifying problem", currentLoggedOnUser.UserId, currentLoggedOnUser.Username, userMod);
                    saveLogRecord(logtoSave);
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
            //if the user is instructor, remove instructor from instructor table
            if (selectedUser.LevelId == 3)
            {
                foreach (var instructor in db.Instructors)
                {
                    if(instructor.FName == selectedUser.FirstName && instructor.LName == selectedUser.LastName)
                    {
                        db.Instructors.Remove(instructor);
                    }
                }
            }
            db.Users.RemoveRange(db.Users.Where(t => t.UserId == selectedUser.UserId));
            
            int saveSuccess = db.SaveChanges();
            if (saveSuccess == 1)
            {
                MessageBox.Show("User Deleted Successfully.", "Delete from Database", MessageBoxButton.OK, MessageBoxImage.Information);
                logtoSave = loggingProcess.CreateAdminLogEntry("Database", "Successfully deleted", currentLoggedOnUser.UserId, currentLoggedOnUser.Username,selectedUser);
                saveLogRecord(logtoSave);
                refreshUserList();
                clearUserDetails();
                stkUserDetails.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Problem Deleting User record.", "Delete Database", MessageBoxButton.OK, MessageBoxImage.Warning);
                logtoSave = loggingProcess.CreateAdminLogEntry("Database", "Deleting problem", currentLoggedOnUser.UserId, currentLoggedOnUser.Username,selectedUser);
                saveLogRecord(logtoSave);
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

        private int saveInstructorRecord(GymLibrary.Instructor insToSave)
        {
            int saveSuccess = 0;
            try
            {
                db.Entry(insToSave).State = System.Data.Entity.EntityState.Added;
                saveSuccess = db.SaveChanges();
            }
            catch (Exception)
            {
                MessageBox.Show("Error saving Instructor record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return saveSuccess;
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
            int totalHeartRate = 0;
            int averageHeartRate = 0;
            double totalHeight = 0;
            double averageHeight = 0;
            double totalWeight = 0;
            double averageWeight = 0;
            double totalBMI = 0;
            double averageBMI = 0;
            int managerCount = 0;
            int instructorCount = 0;
            int adminCount = 0;
            int nutCount = 0;
            int dbcatCount = 0;
            int loginCatCount = 0;

            int evaluationCount = db.Evaluations.Count();
            int clientCount = db.Clients.Count();

            //summary option
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
                    //find the user with user id logged

                    User logUser = findUserByUserId(log.UserId);
                    recordCount++;
                    output = output + Environment.NewLine + $"Record {recordCount} is for Log created by {logUser.FirstName} {logUser.LastName} " +
                        $"whose UserId is {log.UserId}, " + Environment.NewLine +
                        $"log was created on  {log.Date} " + Environment.NewLine +
                        $" Log is registered for the category of {log.Category}" +
                        $" with description of {log.Description} " + Environment.NewLine;

                }
                output = output + Environment.NewLine + $"Total Logs  in the Database is  {recordCount}." + Environment.NewLine;
                tbkAnalysis.Text = output;
            }

            //Count Option
            if (analysisType == AnalysisType.Count && tableSelected == TableSelected.Client)
            {
                foreach (var client in clients)
                {
                    recordCount++;
                }
                output = output + Environment.NewLine + $"Total Client  Records in the Database  is {recordCount}" + Environment.NewLine;
                tbkAnalysis.Text = output;
            }

            if (analysisType == AnalysisType.Count && tableSelected == TableSelected.User)
            {
                foreach (var user in users)
                {
                    recordCount++;
                }
                output = output + Environment.NewLine + $"Total User  Records in the Database  is {recordCount}" + Environment.NewLine;
                tbkAnalysis.Text = output;
            }

            if (analysisType == AnalysisType.Count && tableSelected == TableSelected.Log)
            {
                foreach (var log in logs)
                {
                    recordCount++;
                }
                output = output + Environment.NewLine + $"Total Log  Records in the Database  is {recordCount}" + Environment.NewLine;
                tbkAnalysis.Text = output;
            }

            //Statistics option
            if (analysisType == AnalysisType.Statistics && tableSelected == TableSelected.Client)
            {

                foreach (var evaluation in db.Evaluations)
                {

                    totalHeight = totalHeight + evaluation.Height;
                    totalWeight = totalWeight + evaluation.Weight;
                    totalBMI = totalBMI + evaluation.BMI;
                    totalHeartRate = totalHeartRate + evaluation.HeartRate;

                }
                averageHeartRate = totalHeartRate / evaluationCount;
                averageBMI = totalBMI / evaluationCount;
                averageHeight = totalHeight / evaluationCount;
                averageWeight = totalWeight / evaluationCount;
                output = output + Environment.NewLine + $"The average Heart Rate of evaluated clients is  {averageHeartRate}" + Environment.NewLine;
                output = output + Environment.NewLine + $"The average Height of evaluated clients is  {averageHeight} Feet" + Environment.NewLine;
                output = output + Environment.NewLine + $"The average Weight of evaluated clients is  {averageWeight} KG" + Environment.NewLine;
                output = output + Environment.NewLine + $"The average BMI of evaluated clients is  {averageBMI}" + Environment.NewLine;
                tbkAnalysis.Text = output;
            }

            if (analysisType == AnalysisType.Statistics && tableSelected == TableSelected.User)
            {
                foreach (var user in users)
                {
                    if (user.LevelId == 4)
                    {
                        managerCount++;
                    }
                    if (user.LevelId == 3)
                    {
                        instructorCount++;
                    }
                    if (user.LevelId == 1)
                    {
                        nutCount++;
                    }
                    if (user.LevelId == 2)
                    {
                        adminCount++;
                    }

                }
                output = output + Environment.NewLine + $"The total number of Manager users is  {managerCount}" + Environment.NewLine;
                output = output + Environment.NewLine + $"The total number of Instructor users is  {instructorCount}" + Environment.NewLine;
                output = output + Environment.NewLine + $"The total number of Nutritionist users is  {nutCount}" + Environment.NewLine;
                output = output + Environment.NewLine + $"The total number of Admin users is  {adminCount}" + Environment.NewLine;
                
                tbkAnalysis.Text = output;
            }

            if (analysisType == AnalysisType.Statistics && tableSelected == TableSelected.Log)
            {
                foreach (var log in logs)
                {
                    if (string.Equals(log.Category, "database", StringComparison.CurrentCultureIgnoreCase))
                    {
                        dbcatCount++;
                    }
                    if ((string.Equals(log.Category, "login", StringComparison.CurrentCultureIgnoreCase)) || log.Category.Contains("login" ) || log.Category.Contains("Login"))
                    {
                        loginCatCount++;
                    }
                }

                output = output + Environment.NewLine + $"The total number of Database logs is  {dbcatCount}" + Environment.NewLine;
                output = output + Environment.NewLine + $"The total number of Login logs is  {loginCatCount}" + Environment.NewLine;
                tbkAnalysis.Text = output;


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
                MessageBox.Show("Error saving Log record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            return saveSuccess;
        }

        private User findUserByUserId(int userId)
        {
            User foundUser = new User();
            foreach (var user in db.Users)
            {
                if (user.UserId == userId)
                {
                    foundUser = user;
                }

            }
            return foundUser;
        }
    }
}
