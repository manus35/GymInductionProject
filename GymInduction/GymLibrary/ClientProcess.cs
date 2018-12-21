using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymLibrary
{
    public class ClientProcess
    {
        /// <summary>
        /// validates Client Input
        /// </summary>
        /// <param name="tbxFirstName">
        /// contains Client FirstName</param>
        /// <param name="tbxLastName">
        /// contains client last name</param>
        /// <param name="tbxDateOfBirth">
        /// client dateofbirth</param>
        /// <param name="tbxPhoneNumber">
        /// contains client phone</param>
        /// <param name="tbxGender">
        /// client gender</param>
        /// <returns></returns>
        public bool validateClientInput(string tbxFirstName,string tbxLastName,string tbxDateOfBirth,string tbxPhoneNumber,string tbxGender)
        {
             bool validated = true;

            if (tbxFirstName.Length == 0 || tbxFirstName.Length > 50)
            {
                validated = false;
            }
            if (tbxLastName.Length == 0 || tbxLastName.Length > 50)
            {
                validated = false;
            }
            //in format 12/01/1999 issue with 0000 added when saved
            if (tbxDateOfBirth.Length == 0 || tbxDateOfBirth.Length > 20)
            {
                validated = false;
            }
            if (tbxPhoneNumber.Length == 0 || tbxPhoneNumber.Length > 30)
            {
                validated = false;
            }
            if (tbxGender.Length == 0 || tbxGender.Length > 30)
            {
                validated = false;
            }
            return validated;
        }

        /// <summary>
        /// Validates Induction input
        /// </summary>
        /// <param name="tbxIndClientId">
        /// ContainsInduction client Id</param>
        /// <param name="tbxDate">
        /// Contains Induction Date</param>
        /// <param name="tbxTime">
        /// Contains Induction Time</param>
        /// <param name="tbxStatus">
        /// Contains Induction Status</param>
        /// <param name="cmbIndInsId">
        /// Index of Instructor Id</param>
        /// <param name="instructorList">
        /// Number of records in Instructor List</param>
        /// <returns></returns>
        public bool validateInductionInput(string tbxIndClientId, string tbxDate,
            string tbxTime,string tbxStatus,int cmbIndInsId,int instructorList)
        {
            bool validated = true;


            if (tbxIndClientId.Length == 0)
            {
                validated = false;
            }
            //in format 12/01/1999
            if (tbxDate.Length == 0 || tbxDate.Length > 20)
            {
                validated = false;
            }
            if (tbxTime.Length == 0 || tbxTime.Length > 16)
            {
                validated = false;
            }
            if (tbxStatus.Length == 0 || tbxStatus.Length > 30)
            {
                validated = false;
            }
            if (cmbIndInsId < 0 || cmbIndInsId > instructorList - 1)
            {
                validated = false;
            }


            return validated;
        }

        public void addClientToList(List<Client> clientList,Client client)
        {
            bool validated = true;
            if(string.IsNullOrWhiteSpace(client.FirstName)|| string.IsNullOrWhiteSpace(client.LastName)
                || string.IsNullOrWhiteSpace(client.Gender)) 
            {
                validated = false;
            }
            if(client.ClientId <= 0)
            {
                validated = false;
            }
                
            
           
           
            if (validated)
            {
                clientList.Add(client);
            }
        }


        
       



    }
}
