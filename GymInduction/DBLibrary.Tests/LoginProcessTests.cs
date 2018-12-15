using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GymLibrary;
using Xunit;

namespace DBLibrary.Tests
{
    public class LoginProcessTests
    {
        [Theory]
        [InlineData("d","r",true)]
        [InlineData("", "r", false)]
        [InlineData("ttt", "ooo", true)]
        [InlineData("-ttt", "password", false)]
        [InlineData("tttttttttttttttttttttttttttttttttttt" +
            "ttttttttttttttttttttttttttttttttttttt" +
            "tttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt", "password", false)]
        public void validateUserInput_StringsShouldVerify(string username,string password,bool expected)
        {
            //arrangephase
            LoginProcess loginProcess = new LoginProcess();
            

            //action
            bool actual = loginProcess.ValidateUserInput(username,password);



            //asert
            Assert.Equal(expected,actual);


        }
        
    }
}
