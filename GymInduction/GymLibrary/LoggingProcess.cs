using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymLibrary
{
    public class LoggingProcess
    {

        /// <summary>
        /// Creates a log entry for any action performed on a client such as add,modify and delete
        /// </summary>
        /// <param name="category">
        /// Caegory of log to be recorded</param>
        /// <param name="description">
        /// description of the log to be recorded including action and what performed on</param>
        /// <param name="userId">
        /// userid of user performing the action</param>
        /// <param name="username">
        /// username of user performing the action</param>
        /// <param name="performedClient">
        /// Object that the user performed the action on</param> 
        public Log CreateClientLogEntry(String category, String description, int userId, String username, GymLibrary.Client performedClient)
        {
            string comment = $"Client {performedClient.FullName} {description} by the user  = {username}";

            Log log = new Log();
            if (userId > 0)
            {
                log.UserId = userId;
            }
            log.Category = category;
            log.Description = comment;
            log.Date = DateTime.Now;
            return log;
        }

        //to record induction logs
        public Log CreateInductionLogEntry(String category, String description, int userId, String username, GymLibrary.Induction performedInduction)
        {
            string comment = $"Induction {performedInduction.InductionId} {description} by the user  = {username}";

            Log log = new Log();
            if (userId > 0)
            {
                log.UserId = userId;
            }
            log.Category = category;
            log.Description = comment;
            log.Date = DateTime.Now;
            return log;

        }

        //to record evaluation logs
        public Log CreateEvaluationLogEntry(String category, String description, int userId, String username, GymLibrary.Evaluation performedEvaluation)
        {
            string comment = $"Induction {performedEvaluation.EvaluationId} {description} by the user  = {username}";

            Log log = new Log();
            if (userId > 0)
            {
                log.UserId = userId;
            }
            log.Category = category;
            log.Description = comment;
            log.Date = DateTime.Now;
            return log;

        }

        public Log CreateLoginLogEntry(String category, String description, int userId, String username)
        {
            Log log = new Log();

            string comment = "";
            if (userId >= 0)
            {
                comment = $"{description} user   = {username}";

                log.Category = category;
                log.Description = comment;
                log.UserId = userId;
                log.Date = DateTime.Now;
    
            }

            return log;
        }

        /// <summary>
        /// Creates a log entry for any action performed on a user such as add,modify and delete
        /// </summary>
        /// <param name="category">
        /// Caegory of log to be recorded</param>
        /// <param name="description">
        /// description of the log to be recorded including action and what performed on</param>
        /// <param name="userId">
        /// userid of user performing the action</param>
        /// <param name="username">
        /// username of user performing the action</param>
        /// <param name="performedUser">
        /// Object that the user performed the action on</param>
        public Log CreateAdminLogEntry(String category, String description, int userId, String username, User performedUser)
        {
            string comment = $" {performedUser.Username} {description} by the user  = {username}";

            Log log = new Log();
            if (userId > 0)
            {
                log.UserId = userId;
            }
            log.Category = category;
            log.Description = comment;
            log.Date = DateTime.Now;

            return log;

        }

       

    }
}
