using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DBLibrary;
using GymLibrary;

namespace DBLibrary.Tests
{
    public class ClientProcessTests
    {
        [Theory]
        [InlineData("5","10/09/2019","12:55","Done",int.MinValue,int.MinValue,false)]
        [InlineData("5", "10/09/2019", "12:55", "Done", 1, 0, false)]
        [InlineData("5", "10/09/2019", "12:55", "Donejjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjj" +
            "jjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjj", 0, 0, false)]

        public void ValidateInductionInput(string tbxIndClientId, string tbxDate,
            string tbxTime, string tbxStatus, int cmbIndInsId, int instructorList,bool expected)
        {
            //arange
            ClientProcess clientProcess = new ClientProcess();

            //act
            bool actual = clientProcess.validateInductionInput(tbxIndClientId,tbxDate,
            tbxTime, tbxStatus,cmbIndInsId, instructorList);

            //assert
            Assert.True(actual == false);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("jhj", "jjhjh", "09/09/2007", "hjhj","jhjj", true )]
        [InlineData("Billy", "Murphy", "09/09/1966", "0987656789", "male", true)]
        [InlineData("Susan", "Fisher", "09/02/1982", "0983456789", "male", true)]       
        public void ValidateClientInput(string tbxFirstName,string tbxLastName,string tbxDateOfBirth,string tbxPhoneNumber,string tbxGender, bool expected)
        {
            //arange
            ClientProcess clientProcess = new ClientProcess();

            //act
            bool actual = clientProcess.validateClientInput(tbxFirstName,tbxLastName,tbxDateOfBirth,tbxPhoneNumber,tbxGender);

            //assert
            Assert.True(actual == true);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("jhj", "", "09/09/2007", "hjhj", "jhjj", false)]
        [InlineData("", "", "09/09/1966", "0987656789", "male", false)]
        [InlineData("Susan", "Fisherjkjkjiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii" +
            "hjhjhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh", "09/02/1998", "0983456789", "male", false)]
        [InlineData("Susan", "Fisher", "09/02/1989987", "0983456789", "male", false)]
        public void ValidateClientInputFails(string tbxFirstName, string tbxLastName, string tbxDateOfBirth, string tbxPhoneNumber, string tbxGender, bool expected)
        {
            //arange
            ClientProcess clientProcess = new ClientProcess();

            //act
            bool actual = clientProcess.validateClientInput(tbxFirstName, tbxLastName, tbxDateOfBirth, tbxPhoneNumber, tbxGender);

            //assert
            Assert.True(actual == false);
            Assert.Equal(expected, actual);
        }



        [Fact]
        public void addClientToClientList_ShouldWork()
        {
            //arrangephase
            ClientProcess clientProcess = new ClientProcess();
            Client client = new Client { ClientId = 1, FirstName = "bob", LastName = "Jones", DateOfBirth = Convert.ToDateTime("09/09/1999"), PhoneNumber = 9998887, Gender = "male" };
             List<Client> clients = new List<Client>();

            //action
            clientProcess.addClientToList(clients,client);


            //asert
            Assert.True(clients.Count == 1);
            Assert.Contains<Client>(client,clients);

        }

        [Theory]
        [InlineData(1, "", "burns", "09/09/88", 877878, "jhhj", false)]
        [InlineData(1, "", "", "09/09/2019", 877878, "jhhj", false)]
        [InlineData(5, "jim", "", "09/09/88" , 877878, "jhhj", false)]
        
        public void addClientToClientList_ShouldFail(int ClientId,string FirstName,string LastName,string DateOfBirth,int PhoneNumber,string Gender,bool expected)
        {
            //arrangephase
            ClientProcess clientProcess = new ClientProcess();
            Client client = new Client { ClientId = ClientId, FirstName = FirstName, LastName = LastName,
                DateOfBirth = Convert.ToDateTime(DateOfBirth), PhoneNumber = PhoneNumber, Gender = Gender};
            List<Client> clients = new List<Client>();

            //action
            clientProcess.addClientToList(clients, client);


            //asert
            Assert.True(clients.Count == 0);
            Assert.DoesNotContain<Client>(client, clients);

        }

       

    }
}
