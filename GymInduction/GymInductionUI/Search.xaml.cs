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
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : Page
    {
        GymDbEntities db = new GymDbEntities("metadata=res://*/GymModel.csdl|res://*/GymModel.ssdl|res://*/GymModel.msl;provider = System.Data.SqlClient; provider connection string='data source = 192.168.1.110; initial catalog = GymDb; user id = GymUser; password=Pass.00*;pooling=False;MultipleActiveResultSets=True;App=EntityFramework'");
        List<GymLibrary.Client> clients = new List<GymLibrary.Client>();
        List<Induction> inductions = new List<Induction>();
        List<Instructor> instructors = new List<Instructor>();

        public Search()
        {
            InitializeComponent();
        }

        private void submenuAddUser_Click(object sender, RoutedEventArgs e)
        {
            Client client = new Client();

            client.Show();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            lstClientDetails.ItemsSource = clients;
            lstInductionDetails.ItemsSource = inductions;
            //lstInductionDetails.ItemsSource = instructors;
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
        }
        
        
    }
}
