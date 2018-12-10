using GymLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
using static GymLibrary.GymDbEntities;

namespace GymInductionUI
{
    /// <summary>
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : Page
    {
        GymDbEntities db = new GymDbEntities("metadata=res://*/GymModel.csdl|res://*/GymModel.ssdl|res://*/GymModel.msl;provider = System.Data.SqlClient; provider connection string='data source = 192.168.1.110; initial catalog = GymDb; user id = GymUser; password=Pass.00*;pooling=False;MultipleActiveResultSets=True;App=EntityFramework'");
        List<GymLibrary.Client> clients = new List<GymLibrary.Client>();
        List<Induction> inductions = new List<Induction>();
        List<Instructor> instructors = new List<Instructor>();
        List<Evaluation> evaluations = new List<Evaluation>();
        User instructor = new User();
        


       



        public Search()
        {
            InitializeComponent();
        }

        public Search(User user)
        {
            this.instructor = user;
            InitializeComponent();
            lstClientDetails.Items.Refresh();
            MessageBox.Show(user.Username.ToString());

        }


        private void submenuAddUser_Click(object sender, RoutedEventArgs e)
        {
            Client client = new Client();

            client.Show();
        }
        /*
        private void refreshUserList()
        {
            lstClientDetails.ItemsSource = results;
            results.Clear();
            foreach (var user in db.Users)
            {
                lstClientDetails.Add(user);
            }
            lstClientDetails.Items.Refresh();
        }
        */

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            lstClientDetails.Items.Refresh();
            inductions.Clear();
            instructors.Clear();
            clients.Clear();

            //lstClientDetails.ItemsSource = clients;
            //lstClientDetails.ItemsSource = clients;
            //lstInductionDetails.ItemsSource = inductions;
            //lstInstructorDetails.ItemsSource = instructors;
            //tbxDate.ItemSource = instructors
            foreach (var client in db.Clients)
            {
                clients.Add(client);
            }
            foreach (var induction in db.Inductions)
            {
                inductions.Add(induction);
            }
            foreach (var instructor in db.Instructors)
            {
                instructors.Add(instructor);
            }


            {
                //use the  complex object to get client Induction from 3 different tables
                var results = from i in inductions
                              from n in instructors
                              from c in clients
                              where i.ClientId == c.ClientId && n.InstructorId == i.InstructorId
                              select new ComplexClientInductions()
                              {
                                  FirstName = c.FirstName,
                                  LastName = c.LastName,
                                  DateOfBirth = Convert.ToString(c.DateOfBirth),
                                  PhoneNumber = c.PhoneNumber,
                                  Gender = c.Gender,
                                  indDate = Convert.ToString(i.Date),
                                  indTime = Convert.ToString(i.Time),
                                  indStatus = i.Status,
                                  insFname = n.FName,
                                  insLname = n.LName
                              };
                int r = results.Count();
                MessageBox.Show(r.ToString());
                lstClientDetails.ItemsSource = results;

            }
        }

        [ComplexType]
        public class ComplexClientInductions
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string DateOfBirth { get; set; }
            public int PhoneNumber { get; set; }
            public string Gender { get; set; }
            public string indDate { get; set; }
            public string indTime { get; set; }
            public string indStatus { get; set; }
            public string insFname { get; set; }
            public string insLname { get; set; }
        }


    }
}
