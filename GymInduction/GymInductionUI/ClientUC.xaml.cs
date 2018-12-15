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
    /// Interaction logic for ClientUC.xaml
    /// </summary>
    public partial class ClientUC : UserControl

    {

        //connection string
        GymDbEntities db = new GymDbEntities("metadata=res://*/GymModel.csdl|res://*/GymModel.ssdl|res://*/GymModel.msl;provider = System.Data.SqlClient; provider connection string='data source = 192.168.1.110; initial catalog = GymDb; user id = GymUser; password=Pass.00*;pooling=False;MultipleActiveResultSets=True;App=EntityFramework'");
        ClientProcess clientProcess = new ClientProcess();


        //instance variables
        int delSuccess;
        int clientIdToMatch;
        bool isValidated,validated;
        GymLibrary.Client selectedClient = new GymLibrary.Client();
        GymLibrary.User currentUser = new GymLibrary.User();
        GymLibrary.Induction selectedInduction = new GymLibrary.Induction();
        GymLibrary.Evaluation selectedEvaluation = new GymLibrary.Evaluation();
        List<GymLibrary.Client> clients = new List<GymLibrary.Client>();
        List<GymLibrary.Induction> inductions = new List<GymLibrary.Induction>();
        List<GymLibrary.Evaluation> evaluations = new List<GymLibrary.Evaluation>();
        List<GymLibrary.Instructor> instructors = new List<GymLibrary.Instructor>();

        //enums for all actions
        enum DBOperation
        {
            AddClient,AddInduction,AddEvaluation, ModifyClient,ModifyInduction,ModifyEvaluation
        }
        DBOperation dbOperation = new DBOperation();
        public ClientUC(User user)
        {
            //passed in user to check permisions
            this.currentUser = user;
            
            InitializeComponent();
        }

        private void submenuDelClient_Click(object sender, RoutedEventArgs e)
        {
            bool inductionExists = false;
            bool evaluationExists = false;
            //find out where records exist
            foreach (var induction in inductions)
            {
                if(selectedClient.ClientId == induction.ClientId)
                {
                    inductionExists = true;
                }
            }

            foreach (var evaluation in evaluations)
            {
                if (selectedClient.ClientId == evaluation.ClientId)
                {
                    evaluationExists = true;
                }
            }
            
            //if no constraints delete client record only
            if(!inductionExists && !evaluationExists)
            {
                db.Clients.RemoveRange(db.Clients.Where(t => t.ClientId == selectedClient.ClientId));
                delSuccess = db.SaveChanges();
                if (delSuccess == 1)
                {
                    MessageBox.Show("Client Deleted Successfully.", "Delete from Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    //CreateLogEntry("Database", "Client deleted Successfully", currentUser.UserId, currentUser.Username);
                    refreshListViews();
                    clearClientDetails();
                    stkClientDetails.Visibility = Visibility.Collapsed;

                }
                else
                {
                    MessageBox.Show("Problem Deleting Client record.", "Delete Database", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CreateLogEntry("Database", "Problem deleting client", currentUser.UserId, currentUser.Username);
                }
            }
            else
            {
                
                //delete client and all associated records, start with related 
                db.Inductions.RemoveRange(db.Inductions.Where(t => t.ClientId == selectedClient.ClientId));
                delSuccess = db.SaveChanges();
                if ( evaluationExists || inductionExists)
                {
                    
                    db.Evaluations.RemoveRange(db.Evaluations.Where(t => t.ClientId == selectedClient.ClientId));
                    delSuccess = db.SaveChanges();
                    if (delSuccess == 1 || !evaluationExists)
                    {
                        db.Clients.RemoveRange(db.Clients.Where(t => t.ClientId == selectedClient.ClientId));
                        delSuccess = db.SaveChanges();
                        if (delSuccess == 1)
                        {
                            MessageBox.Show("Client Deleted Successfully.", "Delete from Database", MessageBoxButton.OK, MessageBoxImage.Information);
                            //CreateLogEntry("Database", "Client deleted Successfully", currentUser.UserId, currentUser.Username);
                            refreshListViews();
                            clearClientDetails();
                            stkClientDetails.Visibility = Visibility.Collapsed;

                        }
                        else
                        {
                            MessageBox.Show("Problem Deleting Client record.", "Delete Database", MessageBoxButton.OK, MessageBoxImage.Warning);
                            CreateLogEntry("Database", "Problem deleting client", currentUser.UserId, currentUser.Username);
                        }


                    }
                }
            }

          
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {

            isValidated = clientProcess.validateClientInput(tbxFirstName.Text,tbxLastName.Text,tbxDateOfBirth.Text,tbxPhoneNumber.Text,tbxGender.Text); //need to do for others

            if (!isValidated)
            {
                MessageBox.Show("Error with your input. Please check and try again", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            if (dbOperation == DBOperation.AddClient && isValidated)
            {

                //get  client text input
                GymLibrary.Client clientToadd = new GymLibrary.Client();
                clientToadd.FirstName = tbxFirstName.Text.Trim();
                clientToadd.LastName = tbxLastName.Text.Trim();
                clientToadd.DateOfBirth = Convert.ToDateTime(tbxDateOfBirth.Text.Trim());
                clientToadd.PhoneNumber = Convert.ToInt32(tbxPhoneNumber.Text.Trim());
                clientToadd.Gender = tbxGender.Text.Trim();

                int clientSaved = saveClientRecord(clientToadd);
                if (clientSaved == 1)
                {
                    refreshListViews();
                    MessageBox.Show("Client saved Successfully.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    CreateLogEntry("Database", "Client save Successfully", currentUser.UserId, currentUser.Username);
                    clearClientDetails();


                }
                else
                {
                    MessageBox.Show("Problem saving Client record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CreateLogEntry("Database", "Client save problem", currentUser.UserId, currentUser.Username);
                }
            }

            if (dbOperation == DBOperation.ModifyClient)
            {
                foreach (var client in db.Clients.Where(t => t.ClientId == selectedClient.ClientId))
                {
                    //get  client text input

                    client.FirstName = tbxFirstName.Text.Trim();
                    client.LastName = tbxLastName.Text.Trim();
                    client.DateOfBirth = Convert.ToDateTime(tbxDateOfBirth.Text.Trim());
                    client.PhoneNumber = Convert.ToInt32(tbxPhoneNumber.Text.Trim());
                    client.Gender = tbxGender.Text.Trim();
                }
                int saveSuccess = db.SaveChanges();
                if (saveSuccess == 1)
                {
                    MessageBox.Show("Client Modified Successfully.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    CreateLogEntry("Database", "Client Modfy Successfull", currentUser.UserId, currentUser.Username);
                    refreshListViews(); 
                    clearClientDetails();
                    stkClientDetails.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Problem Modifying Client record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CreateLogEntry("Database", "Client Modify problem", currentUser.UserId, currentUser.Username);
                }

            }
        }








            private int saveClientRecord(GymLibrary.Client clientToSave)
        {
            int saveSuccess = 0;
            try
            {
                db.Entry(clientToSave).State = System.Data.Entity.EntityState.Added;
                saveSuccess = db.SaveChanges();
            }
            catch (Exception)
            {
                MessageBox.Show("Error saving Client record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Error);
                CreateLogEntry("Database", "Client save Error", currentUser.UserId, currentUser.Username);
            }
            return saveSuccess;
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

        private int saveInductionRecord(GymLibrary.Induction indToSave)
        {
            int saveSuccess = 0;
            try
            {
                db.Entry(indToSave).State = System.Data.Entity.EntityState.Added;
                saveSuccess = db.SaveChanges();
            }
            catch (Exception)
            {
                MessageBox.Show("Error saving Induction record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return saveSuccess;
        }

        private int saveEvaluationRecord(GymLibrary.Evaluation evlToSave)
        {
            int saveSuccess = 0;
            try
            {
                db.Entry(evlToSave).State = System.Data.Entity.EntityState.Added;
                saveSuccess = db.SaveChanges();
            }
            catch (Exception)
            {
                MessageBox.Show("Error saving Evaluation record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return saveSuccess;
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            checkUserAccess(currentUser);
            refreshListViews();
            refreshInstructorList();

        }

        public bool validateClientInput()
        {
            validated = true;

            if(tbxFirstName.Text.Length == 0 || tbxFirstName.Text.Length > 50)
            {
                validated = false;
            }
            if (tbxLastName.Text.Length == 0 || tbxLastName.Text.Length > 50)
            {
                validated = false;
            }
            if (tbxDateOfBirth.Text.Length == 0 || tbxDateOfBirth.Text.Length > 30)
            {
                validated = false;
            }
            if (tbxPhoneNumber.Text.Length == 0 || tbxPhoneNumber.Text.Length > 30)
            {
                validated = false;
            }
            if (tbxGender.Text.Length == 0 || tbxGender.Text.Length > 30)
            {
                validated = false;
            }
            return validated;
        }

        public bool validateInductionInput()
        {
            validated = true;
            

            if (tbxIndClientId.Text.Length == 0)
            {
                validated = false;
            }
            //note,cant find date type max length
            if (tbxDate.Text.Length == 0 || tbxDate.Text.Length > 20)
            {
                validated = false;
            }
            //note cant find time max length
            if (tbxTime.Text.Length == 0 || tbxTime.Text.Length > 20)
            {
                validated = false;
            }
            if (tbxStatus.Text.Length == 0 || tbxStatus.Text.Length > 30)
            {
                validated = false;
            }
            if (cmbIndInsId.SelectedIndex < 0 || cmbIndInsId.SelectedIndex > cmbIndInsId.Items.Count-1)
            {
                validated = false;
            }


            return validated;
        }

        public bool validateEvaluationInput()
        {
            validated = true;

            if (tbxHeight.Text.Length == 0 || tbxHeight.Text.Length > 6)
            {
                validated = false;
            }

            if (tbxEvlClientId.Text.Length == 0)
            {
                validated = false;
            }
           
            if (tbxWeight.Text.Length == 0 || tbxWeight.Text.Length > 5)
            {
                validated = false;
            }
            if (tbxHeartRate.Text.Length == 0 || tbxHeartRate.Text.Length > 3)
            {
                validated = false;
            }
            if (tbxBoodPressure.Text.Length == 0 || tbxBoodPressure.Text.Length > 10)
            {
                validated = false;
            }
            if (tbxGoal.Text.Length == 0 || tbxGoal.Text.Length > 50)
            {
                validated = false;
            }
            if (tbxCondition.Text.Length == 0 || tbxCondition.Text.Length > 30)
            {
                validated = false;
            }
            if (cmbEvlInsId.SelectedIndex < 0 || cmbEvlInsId.SelectedIndex > cmbEvlInsId.Items.Count - 1)
            {
                validated = false;
            }
            return validated;
        }

        private void refreshListViews()
        {
            lstIndDetails.ItemsSource = inductions;
            lstEvaluationDetails.ItemsSource = evaluations;
            lstClientDetails.ItemsSource = clients;

            inductions.Clear();
            clients.Clear();
            instructors.Clear();
            evaluations.Clear();

            foreach (var induction in db.Inductions)
            {
                inductions.Add(induction);
            }
            lstIndDetails.Items.Refresh();
            foreach (var evaluation in db.Evaluations)
            {
                evaluations.Add(evaluation);
            }
            lstEvaluationDetails.Items.Refresh();


            foreach (var client in db.Clients)
            {
                clients.Add(client);
            }
            lstClientDetails.Items.Refresh();

        }

        private void refreshClientList()
        {
            clients.Clear();

            foreach (var client in db.Clients)
            {
                clientProcess.addClientToList(clients, client);
                //clients.Add(client);
            }
            lstClientDetails.ItemsSource = clients;
            lstIndDetails.ItemsSource = inductions;
            lstEvaluationDetails.ItemsSource = inductions;

        }

        private void refreshInstructorList()
        {
            instructors.Clear();
            evaluations.Clear();
            foreach (var instructor in db.Instructors)
            {
                instructors.Add(instructor);
            }
            cmbIndInsId.ItemsSource = instructors;
            cmbIndInsId.Items.Refresh();
            foreach (var evaluation in db.Evaluations)
            {
                evaluations.Add(evaluation);
            }
            cmbEvlInsId.ItemsSource = instructors;
            cmbEvlInsId.Items.Refresh();
        }

       



        private void submenuAddClient_Click(object sender, RoutedEventArgs e)
        {
            stkClientDetails.Visibility = Visibility.Visible;
            //stkEvaluationDetails.Visibility = Visibility.Visible;
            
            checkUserAccess(currentUser);
            dbOperation = DBOperation.AddClient;
            clearAllTextFields();
        }

        private void submenuModClient_Click(object sender, RoutedEventArgs e)
        {
            stkClientDetails.Visibility = Visibility.Visible;
            checkUserAccess(currentUser);
            //stkEvaluationDetails.Visibility = Visibility.Visible;
            selectedClient = clients.ElementAt(lstClientDetails.SelectedIndex);
            dbOperation = DBOperation.ModifyClient;
            tbxPhoneNumber.Text = selectedClient.PhoneNumber.ToString();
            tbxFirstName.Text = selectedClient.FirstName;
            tbxLastName.Text = selectedClient.LastName;
            tbxGender.Text = selectedClient.Gender;
            tbxDateOfBirth.Text = Convert.ToString(selectedClient.DateOfBirth);
        }

        public int saveClient(GymLibrary.Client client)
        {
            db.Entry(client).State = System.Data.Entity.EntityState.Added;
            int saveStatus = db.SaveChanges();
            return saveStatus;
        }

        private void clearAllTextFields()
        {
            clearClientDetails();
            clearInductionDetails();
            clearEvaluationDetails();
        }

        private void clearClientDetails()
        {
            tbxFirstName.Text = " ";
            tbxLastName.Text = " ";
            tbxDateOfBirth.Text = " ";
            tbxPhoneNumber.Text = " ";
            tbxGender.Text = "";
        }

        private void clearInductionDetails()
        {
            tbxIndClientId.Text = " ";
            //tbxIndInsId.Text = " ";
            tbxDate.Text = " ";
            tbxTime.Text = " ";
            tbxStatus.Text = " ";
            cmbIndInsId.SelectedIndex = -1;
        }

        private void clearEvaluationDetails()
        {
            tbxHeight.Text = " ";
            tbxWeight.Text = " ";
            tbxHeartRate.Text = " ";
            tbxBoodPressure.Text = " ";
            tbxGoal.Text = "";
            tbxCondition.Text = "";
            tbxBMI.Text = "";
            cmbEvlInsId.SelectedIndex = -1;
        }

        private void lstEvaluationDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
                cmbEvlInsId.Items.Refresh();
            checkUserAccess(currentUser);
            if (lstEvaluationDetails.SelectedIndex > 0)
                {
                    selectedEvaluation = evaluations.ElementAt(lstEvaluationDetails.SelectedIndex);


                    if (dbOperation == DBOperation.AddEvaluation)
                    {
                        clearEvaluationDetails();
                    }
                    if (dbOperation == DBOperation.ModifyEvaluation)
                    {
                    checkUserAccess(currentUser);
                    tbxEvlClientId.Text = selectedClient.ClientId.ToString();
                    tbxHeight.Text = selectedEvaluation.Height.ToString();
                    tbxWeight.Text = selectedEvaluation.Weight.ToString();
                    tbxHeartRate.Text = selectedEvaluation.HeartRate.ToString();
                    tbxBoodPressure.Text = selectedEvaluation.BloodPressure.ToString();
                    tbxBMI.Text = selectedEvaluation.BMI.ToString();
                    tbxGoal.Text = selectedEvaluation.Goal.ToString();
                    tbxCondition.Text = selectedEvaluation.Condition.ToString();
                    cmbIndInsId.SelectedIndex = selectedEvaluation.InstructorId - 1;


                    }

                }
            }

        private void lstClientDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            checkUserAccess(currentUser);
            if (lstClientDetails.SelectedIndex > 0)
            {
                selectedClient = clients.ElementAt(lstClientDetails.SelectedIndex);
                

                if (dbOperation == DBOperation.AddClient)
                {
                    clearClientDetails();
                }
                if (dbOperation == DBOperation.ModifyClient)
                {
                    tbxPhoneNumber.Text = selectedClient.PhoneNumber.ToString();
                    tbxFirstName.Text = selectedClient.FirstName;
                    tbxLastName.Text = selectedClient.LastName;
                    tbxGender.Text = selectedClient.Gender;
                    tbxDateOfBirth.Text = Convert.ToString(selectedClient.DateOfBirth);
                }

            }

        }
        /// <summary>
        /// Links and joins tables from the db for listviews
        /// </summary>
        /// <param name="inductionId">
        /// id of induction holds clientid and instructorid
        /// </param>
        
        /// <returns>
        /// void sets Listviews of client,induction,evaluation tabs
        /// </returns>
        private void populateClientTabs(int inductionId)
        {
            var allClientInfo = from _induction in db.Inductions.Where(t => t.InductionId == inductionId)

                                join _client in db.Clients on _induction.ClientId equals _client.ClientId
                                join _evaluation in db.Evaluations on _induction.ClientId equals _evaluation.ClientId
                                join _instructor in db.Instructors on _induction.InstructorId equals _instructor.InstructorId

                                select new
                                {
                                    _client.ClientId,
                                    _client.FirstName,
                                    _client.LastName,
                                    _client.PhoneNumber,
                                    _client.Gender,
                                    _client.DateOfBirth,
                                    _induction.Date,
                                    _induction.InductionId,
                                    _induction.Time,
                                    _induction.Status,
                                    _instructor.FName,
                                     _instructor.LName,
                                    _instructor.InstructorId,
                                    
                                    _evaluation.EvaluationId,
                                    _evaluation.Height,
                                    _evaluation.Weight,
                                    _evaluation.HeartRate,
                                    _evaluation.BloodPressure,
                                    _evaluation.Goal,
                                    _evaluation.Condition,
                                    _evaluation.BMI

                                };

            foreach (var record in allClientInfo)
            {
                if (record.ClientId > 0)
                {
                    checkUserAccess(currentUser);


                    tbxPhoneNumber.Text = record.PhoneNumber.ToString();
                    tbxFirstName.Text = record.FirstName;
                    tbxLastName.Text = record.LastName;
                    tbxGender.Text = record.Gender;
                     tbxDateOfBirth.Text = Convert.ToString(record.DateOfBirth);
                }
                if (record.InductionId > 0)
                {
                    checkUserAccess(currentUser);
                    tbxDate.Text = record.Date.ToString();
                    tbxTime.Text = record.Time.ToString();
                    tbxStatus.Text = record.Status;
                    



                }
                
                
                if (record.EvaluationId > 0)
                {
                    checkUserAccess(currentUser);
                    tbxBoodPressure.Text = record.BloodPressure;
                    tbxHeartRate.Text = record.HeartRate.ToString();
                    tbxHeight.Text = record.Height.ToString();
                    tbxWeight.Text = record.Weight.ToString();
                    tbxCondition.Text = record.Condition;
                    tbxGoal.Text = record.Goal;
                    tbxBMI.Text = record.BMI.ToString();
                }

            }


                }

       

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            stkClientDetails.Visibility = Visibility.Collapsed;
            clearClientDetails();
        }

        private void submenuAddInd_Click(object sender, RoutedEventArgs e)
        {
            cmbIndInsId.Items.Refresh();
            refreshInstructorList();
            stkIndDetails.Visibility = Visibility.Visible;

            checkUserAccess(currentUser);
            // tbiEvaluation.IsEnabled = true;
            dbOperation = DBOperation.AddInduction;
            clearAllTextFields();
        }

        private void submenuModInd_Click(object sender, RoutedEventArgs e)
        {
            cmbIndInsId.Items.Refresh();
            refreshInstructorList();
            stkIndDetails.Visibility = Visibility.Visible;
            //stkEvaluationDetails.Visibility = Visibility.Visible;
            selectedInduction = inductions.ElementAt(lstIndDetails.SelectedIndex);
            dbOperation = DBOperation.ModifyInduction;
            tbxDate.Text = selectedInduction.Date.ToString();
            tbxTime.Text = selectedInduction.Time.ToString();
            tbxStatus.Text = selectedInduction.Status;
            tbxIndClientId.Text = selectedInduction.ClientId.ToString();
            //cmbIndInsId.SelectedIndex = selectedInduction.InstructorId+1;
            int insFound = findInstructorId(selectedInduction.ClientId);
            cmbIndInsId.SelectedIndex = insFound;
        }

        private void submenuDelInd_Click(object sender, RoutedEventArgs e)
        {
            db.Inductions.RemoveRange(db.Inductions.Where(t => t.InductionId == selectedInduction.InductionId));
            int saveSuccess = db.SaveChanges();
            if (saveSuccess == 1)
            {
                MessageBox.Show("Induction Record Deleted Successfully.", "Delete from Database", MessageBoxButton.OK, MessageBoxImage.Information);
                refreshListViews();
                clearInductionDetails();
                refreshInstructorList();
                stkIndDetails.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Problem Deleting Induction record.", "Delete Database", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


        }

        private void btnIndCancel_Click(object sender, RoutedEventArgs e)
        {
            stkIndDetails.Visibility = Visibility.Collapsed;
            
            clearInductionDetails();
        }

        private void btnIndOK_Click(object sender, RoutedEventArgs e)
        {
            {


                isValidated = clientProcess.validateInductionInput(tbxIndClientId.Text,tbxDate.Text,tbxTime.Text,tbxStatus.Text,cmbIndInsId.SelectedIndex,instructors.Count);

                if (!isValidated)
                {
                    MessageBox.Show("Error with your input. Please check and try again", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                //check client exists
                clientIdToMatch = Convert.ToInt32(tbxIndClientId.Text.Trim());
                bool clientMatch = false;


                foreach (var client in db.Clients)
                {
                    if (client.ClientId == clientIdToMatch)
                    {
                        clientMatch = true;
                    }
                }

                if (!clientMatch)
                {
                    MessageBox.Show("Client does not exist in Database. Please add client details before adding Induction", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Error);
                }


                if (dbOperation == DBOperation.AddInduction && isValidated && clientMatch == true)
                {


                    //get  Induction text input

                    GymLibrary.Induction inductionToAdd = new GymLibrary.Induction();
                    inductionToAdd.ClientId = Convert.ToInt32(tbxIndClientId.Text.Trim());

                    inductionToAdd.InstructorId = cmbIndInsId.SelectedIndex+1;
                    
                    inductionToAdd.Date = Convert.ToDateTime(tbxDate.Text.Trim());
                    inductionToAdd.Time = TimeSpan.Parse(tbxTime.Text.Trim());
                    inductionToAdd.Status = tbxStatus.Text.Trim();


                    int indSaved = saveInductionRecord(inductionToAdd); 
                    if (indSaved == 1)
                    {
                        refreshListViews();
                        MessageBox.Show("Induction saved Successfully.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Information);
                        clearInductionDetails();
                        stkIndDetails.Visibility = Visibility.Collapsed;
                        refreshInstructorList();




                    }
                    else
                    {
                        MessageBox.Show("Problem saving Induction record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                if (dbOperation == DBOperation.ModifyInduction && clientMatch)
                {
                    //find inductions with client id
                    clientIdToMatch = Convert.ToInt32(tbxIndClientId.Text.Trim());
                    foreach (var induction in db.Inductions)
                    {
                        if (induction.ClientId == clientIdToMatch)
                        {
                            clientIdToMatch = induction.InductionId;
                        }
                    }

                    foreach (var induction in db.Inductions.Where(t => t.InductionId == clientIdToMatch))
                    {
                        
                        //get  induction text input
                        //make sure loop doesnt alter id
                        induction.InstructorId = 0;
                        induction.ClientId = Convert.ToInt32(tbxIndClientId.Text.Trim());
                        induction.InstructorId = cmbIndInsId.SelectedIndex+1;
                        induction.Date = Convert.ToDateTime(tbxDate.Text.Trim());
                        induction.Time = TimeSpan.Parse(tbxTime.Text.Trim());
                        induction.Status = tbxStatus.Text.Trim();

                    }
                    int saveSuccess = db.SaveChanges();

                    if (saveSuccess == 1)
                    {
                        MessageBox.Show("Induction Modified Successfully.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Information);
                        refreshListViews();
                        clearClientDetails();
                        refreshInstructorList();
                        clearInductionDetails();
                        stkClientDetails.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        MessageBox.Show("Problem Modifying Induction record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                }
            }

        }

        private void submenuAddEvaluation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void submenuModEvaluation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void submenuDelEvaluation_Click(object sender, RoutedEventArgs e)
        {
            {
                db.Evaluations.RemoveRange(db.Evaluations.Where(t => t.EvaluationId == selectedEvaluation.EvaluationId));
                int saveSuccess = db.SaveChanges();
                if (saveSuccess == 1)
                {
                    MessageBox.Show("Evaluation Record Deleted Successfully.", "Delete from Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    refreshListViews();
                    clearEvaluationDetails();
                    refreshInstructorList();
                    stkEvaluationDetails.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Problem Deleting Evaluation record.", "Delete Database", MessageBoxButton.OK, MessageBoxImage.Warning);
                }


            }
        }

        private void btnEvlCancel_Click(object sender, RoutedEventArgs e)
        {
            stkEvaluationDetails.Visibility = Visibility.Collapsed;
            clearEvaluationDetails();
        }

        private void btnEvlOK_Click(object sender, RoutedEventArgs e)
        {


            isValidated = validateEvaluationInput();

            if (!isValidated)
            {
                MessageBox.Show("Error with your input. Please check and try again", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //check client exists
            clientIdToMatch = Convert.ToInt32(tbxEvlClientId.Text.Trim());
            bool clientMatch = false;


            foreach (var client in db.Clients)
            {
                if (client.ClientId == clientIdToMatch)
                {
                    clientMatch = true;
                }
            }

            if (!clientMatch)
            {
                MessageBox.Show("Client does not exist in Database. Please add client details before adding Induction", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            if (dbOperation == DBOperation.AddEvaluation && isValidated && clientMatch == true)
            {
                

                //get  Evaluation text input

                //get  Evaluation text input
                GymLibrary.Evaluation evaluationToAdd = new GymLibrary.Evaluation();
                evaluationToAdd.ClientId = Convert.ToInt32(tbxEvlClientId.Text.Trim());
                evaluationToAdd.InstructorId = cmbEvlInsId.SelectedIndex + 1;
                evaluationToAdd.Height = Convert.ToDouble(tbxHeight.Text.Trim());
                evaluationToAdd.Weight = Convert.ToDouble(tbxWeight.Text.Trim());
                evaluationToAdd.HeartRate = Convert.ToInt32(tbxHeartRate.Text.Trim());
                evaluationToAdd.BloodPressure = tbxBoodPressure.Text.Trim();
                evaluationToAdd.Condition = tbxCondition.Text.Trim();
                evaluationToAdd.Goal = tbxGoal.Text.Trim();
                double w = Convert.ToDouble(tbxWeight.Text.Trim());
                double h = Convert.ToDouble(tbxHeight.Text.Trim());
                evaluationToAdd.BMI = calculateBMI(h, w);


                int evlSaved = saveEvaluationRecord(evaluationToAdd);
                if (evlSaved == 1)
                {
                    refreshListViews();
                    MessageBox.Show("Evaluation saved Successfully.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    clearEvaluationDetails();
                    stkEvaluationDetails.Visibility = Visibility.Collapsed;
                    refreshInstructorList();




                }
                else
                {
                    MessageBox.Show("Problem saving Evaluation record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            
            if (dbOperation == DBOperation.ModifyEvaluation && clientMatch)
            {
                
                //find evaluations with client id
                clientIdToMatch = Convert.ToInt32(tbxEvlClientId.Text.Trim());
                foreach (var evaluation in db.Evaluations)
                {
                    if (evaluation.ClientId == clientIdToMatch)
                    {
                        clientIdToMatch = evaluation.EvaluationId;
                    }
                }

                foreach (var evaluation in db.Evaluations.Where(t => t.EvaluationId == clientIdToMatch))
                {

                    //get  induction text input
                    //make sure loop doesnt alter id
                    evaluation.InstructorId = 0;
                    evaluation.ClientId = Convert.ToInt32(tbxEvlClientId.Text.Trim());
                    evaluation.InstructorId = cmbEvlInsId.SelectedIndex + 1;
                    evaluation.Height = Convert.ToDouble(tbxHeight.Text.Trim());
                    evaluation.Weight = Convert.ToDouble(tbxWeight.Text.Trim());
                    evaluation.HeartRate = Convert.ToInt32(tbxHeartRate.Text.Trim());
                    evaluation.BloodPressure = tbxBoodPressure.Text.Trim();
                    evaluation.Condition = tbxCondition.Text.Trim();
                    evaluation.Goal = tbxGoal.Text.Trim();
                    double w = Convert.ToDouble(tbxWeight.Text.Trim());
                    double h = Convert.ToDouble(tbxHeight.Text.Trim());
                    evaluation.BMI = calculateBMI(h, w);


                }
                int saveSuccess = db.SaveChanges();

                if (saveSuccess == 1)
                {
                    MessageBox.Show("Evaluation Modified Successfully.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Information);
                    refreshListViews();
                    clearClientDetails();
                    refreshInstructorList();
                    clearEvaluationDetails();
                    stkEvaluationDetails.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Problem Modifying Evaluation record.", "Save To Database", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
        }

            private void cmbIndInsId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private int findInstructorId(int cliId)
        {
            int instructorMatch = 0;
            foreach (var induction in db.Inductions)
            {
                //instructorMatch = 0;
                if (induction.ClientId == cliId)
                {
                    
                    instructorMatch = induction.InstructorId-1;
                    if(instructorMatch <0)
                    {
                        instructorMatch = 0;
                    }
                   
                    
                }   
            }
            return instructorMatch;
        }
        private void submenuSchedule_Click(object sender, RoutedEventArgs e)
        {
            clearInductionDetails();
            stkIndDetails.Visibility = Visibility.Visible;
            cmbIndInsId.Items.Refresh();
            checkUserAccess(currentUser);
            refreshListViews();
            refreshInstructorList();

            dbOperation = DBOperation.AddInduction;
            tbxIndClientId.Text = selectedClient.ClientId.ToString();
            
            foreach (var ind in db.Inductions)
            {
                if (ind.ClientId == selectedClient.ClientId)
                {
                    dbOperation = DBOperation.ModifyInduction;
                    tbxIndClientId.Text = selectedClient.ClientId.ToString();
                    tbxDate.Text = ind.Date.ToString();
                    tbxTime.Text = ind.Time.ToString();
                    tbxStatus.Text = ind.Status;
                    int insFound = findInstructorId(selectedClient.ClientId);

                    //refreshInstructorList();
                    
                    cmbIndInsId.SelectedIndex = insFound;
                    
                }
            }   
                //clearAllTextFields();
                tabClient.SelectedItem = tbiInduction;
                tbiInduction.Focus();
            }

        /// <summary>
        /// Finds the instructor id matching the client id that the instructor was assigned
        /// </summary>
        /// <param name="cliId">
        /// int clientId of client to associated with the instructor</param>
        /// <returns>
        /// Returns the instructor id matching the client id that the instructor was assigned
        /// </returns>
        private int findEvlInstructorId(int cliId)
        {
            int instructorMatch = 0;
            foreach (var evaluation in db.Evaluations)
            {
                //instructorMatch = 0;
                if (evaluation.ClientId == cliId)
                {

                    instructorMatch = evaluation.InstructorId - 1;
                    if (instructorMatch < 0)
                    {
                        instructorMatch = 0;
                    }

                    
                }
            }
            return instructorMatch;
        }

        private void submenuEvaluation_Click(object sender, RoutedEventArgs e)
        {
            stkEvaluationDetails.Visibility = Visibility.Visible;
            cmbEvlInsId.Items.Refresh();
            if (currentUser.UserId != 2)
            {
                checkUserAccess(currentUser);
            }

            dbOperation = DBOperation.AddEvaluation;
            tbxEvlClientId.Text = selectedClient.ClientId.ToString();
            
            foreach (var evl in db.Evaluations)
            {
                if (evl.ClientId == selectedClient.ClientId)
                {
                    dbOperation = DBOperation.ModifyEvaluation;
                    tbxEvlClientId.Text = selectedClient.ClientId.ToString();
                    tbxHeight.Text = evl.Height.ToString();
                    tbxWeight.Text = evl.Weight.ToString();
                    tbxHeartRate.Text = evl.HeartRate.ToString();
                    tbxBoodPressure.Text = evl.BloodPressure.ToString();
                    tbxBMI.Text = evl.BMI.ToString();
                    tbxGoal.Text = evl.Goal.ToString();
                    tbxCondition.Text = evl.Condition.ToString();


                    int evlFound = findEvlInstructorId(selectedClient.ClientId);

                   
                    cmbEvlInsId.SelectedIndex = evlFound;
                    //MessageBox.Show("in sc.hedule in "+(insFound).ToString());
                }
            }

            tabClient.SelectedItem = tbiEvaluation;
            tbiEvaluation.Focus();
        }


        private void lstIndtDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbIndInsId.Items.Refresh();
            checkUserAccess(currentUser);
            if (lstIndDetails.SelectedIndex > 0)
            {
                selectedInduction = inductions.ElementAt(lstIndDetails.SelectedIndex);


                if (dbOperation == DBOperation.AddInduction)
                {
                    clearInductionDetails();
                }
                if (dbOperation == DBOperation.ModifyInduction)
                {
                    checkUserAccess(currentUser);
                    tbxDate.Text = selectedInduction.Date.ToString();
                    tbxTime.Text = selectedInduction.Time.ToString();
                    tbxStatus.Text = selectedInduction.Status;
                    tbxIndClientId.Text = selectedInduction.ClientId.ToString();
                    cmbIndInsId.SelectedIndex = selectedInduction.InstructorId-1;


                }

            }
        }


        /// <summary>
        /// calculates BMI of client and is used to save directly to the DB
        /// BMI = weightInPounds x 703 / heightInInches x heightInInches
        /// </summary>
        /// <param name="height">
        /// Height of the client in feet and inches
        /// </param>
        /// <param name="weight">
        /// Wweight of the client in KG
        /// </param>
        /// <returns>
        /// BMI of client
        /// </returns>
        private double calculateBMI(double height,double weight)
        {
            double heightInInches = height * 12;
            double weightInpounds = weight * 2.20462;
            double bmi = Math.Round((weightInpounds *703) / (heightInInches* heightInInches),1);
            return bmi;
        }

        /// <summary>
        /// Uses the passed in current user object to set permisions for that user
        /// </summary>
        /// <param name="user">
        /// represents the current user logged in</param>
        /// <returns>
        /// Method is void but hides all content not permitted to the user
        /// </returns>
        private void checkUserAccess(User user)
        {
            if (user.LevelId == 1)
            {

                tbiInduction.IsEnabled = false;
                submenuAddClient.IsEnabled = false;
                submenuDelClient.IsEnabled = false;
                submenuModClient.IsEnabled = false;
                submenuSchedule.IsEnabled = false;
                
                

            }
            if (user.LevelId == 2)
            {
                tbiEvaluation.IsEnabled = false;
                submenuEvaluation.IsEnabled = false;
            }
            if (user.LevelId == 3)
            {
                submenuAddClient.IsEnabled = false;
                submenuDelClient.IsEnabled = false;
                submenuModClient.IsEnabled = false;
            }
        }

        private void CreateLogEntry(String category, String description, int userId, String username)
        {
            string comment = ""+(description) +""+" user credentials  ="+ (username);
            Log log = new Log();
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


    }
           
            }

