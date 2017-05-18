namespace Promact.Erp.DomainModel.Migrations
{
    using ApplicationClass;
    using Models;
    using System;
    using System.Data.Entity.Migrations;

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
            Question firstQuestionTaskMail = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.YourTask, QuestionStatement = "On which task did you work today?", Type = BotQuestionType.TaskMail };
            Question secondQuestionTaskMail = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.HoursSpent, QuestionStatement = "How many hours have you spent on task?", Type = BotQuestionType.TaskMail };
            Question thirdQuestionTaskMail = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.Status, QuestionStatement = "What is the status of task?", Type = BotQuestionType.TaskMail };
            Question fourthQuestionTaskMail = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.Comment, QuestionStatement = "List Comment/Roadblock (if any).", Type = BotQuestionType.TaskMail };
            Question fifthQuestionTaskMail = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.SendEmail, QuestionStatement = "Your task mail is ready. Do you want to send it?", Type = BotQuestionType.TaskMail };
            Question sixthQuestionTaskMail = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.ConfirmSendEmail, QuestionStatement = "After sending the email you won't be able to add any task for today. Are you sure you want to send email?", Type = BotQuestionType.TaskMail };
            Question seventhQuestionTaskMail = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.TaskMailSend, QuestionStatement = "Task Mail Complete", Type = BotQuestionType.TaskMail };
            Question firstQuestionScrumBot = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.Yesterday, QuestionStatement = "What did you do yesterday?", Type = BotQuestionType.Scrum };
            Question secondQuestionScrumBot = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.Today, QuestionStatement = "What are you going to do today?", Type = BotQuestionType.Scrum };
            Question thirdQuestionScrumBot = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.RoadBlock, QuestionStatement = "Any roadblock?", Type = BotQuestionType.Scrum };
            Question eighthQuestionTaskMail = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.RestartTask, QuestionStatement = "Do you want to add another task?", Type = BotQuestionType.TaskMail };
            Question firstQuestionLeaveManagement = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.LeaveType, QuestionStatement = "Which type of leave you want to apply for - cl or sl?", Type = BotQuestionType.LeaveManagement };
            Question secondQuestionLeaveManagement = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.Reason, QuestionStatement = "Mention the reason for your leave", Type = BotQuestionType.LeaveManagement };
            Question thirdQuestionLeaveManagement = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.FromDate, QuestionStatement = "Mention the date from, you want to apply leave", Type = BotQuestionType.LeaveManagement };
            Question fourthQuestionLeaveManagement = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.EndDate, QuestionStatement = "Mention the date upto, you want to apply leave", Type = BotQuestionType.LeaveManagement };
            Question fifthQuestionLeaveManagement = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.RejoinDate, QuestionStatement = "Mention the date, you want to rejoing your duties", Type = BotQuestionType.LeaveManagement };
            Question sixthQuestionLeaveManagement = new Question() { CreatedOn = DateTime.UtcNow, OrderNumber = QuestionOrder.SendLeaveMail, QuestionStatement = "Your leave is ready. Do you want to apply?", Type = BotQuestionType.LeaveManagement };
            context.Question.AddOrUpdate(x => x.OrderNumber,
                firstQuestionTaskMail,
                secondQuestionTaskMail,
                thirdQuestionTaskMail,
                fourthQuestionTaskMail,
                fifthQuestionTaskMail,
                sixthQuestionTaskMail,
                seventhQuestionTaskMail,
                firstQuestionScrumBot,
                secondQuestionScrumBot,
                thirdQuestionScrumBot,
                eighthQuestionTaskMail,
                firstQuestionLeaveManagement,
                secondQuestionLeaveManagement,
                thirdQuestionLeaveManagement,
                fourthQuestionLeaveManagement,
                fifthQuestionLeaveManagement,
                sixthQuestionLeaveManagement
                );
            context.SaveChanges();
        }
    }
}