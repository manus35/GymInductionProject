using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymLibrary
{
    public class LoginProcess
    {

        /// <summary>
        /// Validates the user credentials against those in the database
        /// </summary>
        /// <param name="username">
        /// username entered by user
        /// </param>
        /// <param name="password">
        /// password entered by user
        /// </param>
        /// <returns>
        /// validated user
        /// </returns>
        public bool ValidateUserInput(string username, string password)
        {
            //initialise validated to true and pnly change to false when conditions are met
            bool validated = true;

            //check if username exists and is less than allowed 30 characters
            if (username.Length == 0 || username.Length > 30)
            {
                validated = false;
            }
            //check for unwanted chars in username I will allow numbers for now
            
            foreach(char ch in username )
            {
                if(ch == '-' || ch == '+' || ch == '@' || ch == '#')
                {
                    validated = false;
                }
            }
            
            //password exists and is no more than the allowed 30 characters
            if (password.Length == 0 || password.Length > 30)
            {
                validated = false;
            }
            return validated;

        }





    }
}
