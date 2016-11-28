namespace Promact.Erp.DomainModel.Migrations
{
    using Models;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public class Configuration : DbMigrationsConfiguration<Promact.Erp.DomainModel.Context.PromactErpContext>
    {
        /// <summary>
        /// Enable auto-migration for this project
        /// </summary>
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Promact.Erp.DomainModel.Context.PromactErpContext context)
        {
            if (context.Question.Count() == 0)
            {
                Question firstQuestionTaskMail = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = 1, QuestionStatement = "On which task you worked on Today?", Type = 2 };
                Question secondQuestionTaskMail = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = 2, QuestionStatement = "How many hours you spent on this task?", Type = 2 };
                Question thirdQuestionTaskMail = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = 3, QuestionStatement = "What is the Status of this task?", Type = 2 };
                Question fourthQuestionTaskMail = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = 4, QuestionStatement = "Mention if any Comment/Roadblock.", Type = 2 };
                Question fifthQuestionTaskMail = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = 5, QuestionStatement = "Do you want to send your task mail?", Type = 2 };
                Question sixthQuestionTaskMail = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = 6, QuestionStatement = "Are you sure to send mail? After sending the email you won't be able to add any task for today.", Type = 2 };
                Question seventhQuestionTaskMail = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = 7, QuestionStatement = "Task Mail Complete", Type = 2 };
                Question firstQuestionScrumBot = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = 1, QuestionStatement = "What did you do to change the world yesterday?", Type = 1 };
                Question secondQuestionScrumBot = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = 2, QuestionStatement = "How are you going to rock it today?", Type = 1 };
                Question thirdQuestionScrumBot = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = 3, QuestionStatement = "Are there any obstacles unfortunate enough to be standing in your way?", Type = 1 };
                context.Question.AddOrUpdate(x => x.Id,
                    firstQuestionTaskMail,
                    secondQuestionTaskMail,
                    thirdQuestionTaskMail,
                    fourthQuestionTaskMail,
                    fifthQuestionTaskMail,
                    sixthQuestionTaskMail,
                    seventhQuestionTaskMail,
                    firstQuestionScrumBot,
                    secondQuestionScrumBot,
                    thirdQuestionScrumBot
                    );
                context.SaveChanges();
            }
        }
    }
}
