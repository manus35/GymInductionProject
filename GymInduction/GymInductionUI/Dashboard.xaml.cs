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

        GymDbEntities db = new GymDbEntities("metadata=res://*/GymModel.csdl|res://*/GymModel.ssdl|res://*/GymModel.msl;provider = System.Data.SqlClient; provider connection string='data source = 192.168.1.110; initial catalog = GymDb; user id = GymUser; password=Pass.00*;pooling=False;MultipleActiveResultSets=True;App=EntityFramework'");
        List<GymLibrary.Client> clients = new List<GymLibrary.Client>();
        List<Induction> inductions = new List<Induction>();
        List<Instructor> instructors = new List<Instructor>();
        public User user = new User();
        
        public Client client = new Client();
        public Evaluation evaluation = new Evaluation();
        public Induction induction = new Induction();


        public Dashboard()
        {
            InitializeComponent();
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (user.LevelId == 3)
            {
                Search search = new Search(user);
                frmMain.Navigate(search);
                //add method
            }
            else
            {
                Admin admin = new Admin(user);
                frmMain.Navigate(admin);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Environment.Exit(0);
        }

        private void btnClient_Click(object sender, RoutedEventArgs e)
        {
            ClientUC client = new ClientUC(user);
            frmMain.Navigate(client);


        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Search search = new Search();
            frmMain.Navigate(search);
        }

        private void checkUserAccess(User user)
        {
            if(user.LevelId > 0)
            {
                if (user.LevelId == 4)
                {
                    btnAdmin.Visibility = Visibility.Visible;
                }
                if (user.LevelId == 3)
                {
                    btnAdmin.Content = "Schedule";
                    btnAdmin.Visibility = Visibility.Visible;
                }
            }
            
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            checkUserAccess(user);
        }

        private void getInstructorScheduleDetails()
        {
           
        }
    }
}
