
namespace Promact.Erp.DomainModel.ApplicationClass
{
    /// <summary>
    /// Leave Status
    /// </summary>
    public enum Condition
    {
        Pending,
        Approved,
        Rejected,
        Cancel
    }

    /// <summary>
    /// Slack Slash Command Actions
    /// </summary>
    public enum SlackAction
    {
        apply,
        list,
        cancel,
        status,
        balance,
        help,
        update
    }

    /// <summary>
    /// Daily Task Mail Progress Status
    /// </summary>
    public enum TaskMailStatus
    {
        inprogress,
        completed,
        roadblock,
        none
    }

    /// <summary>
    /// Scrum Status
    /// </summary>
    public enum ScrumStatus
    {
        NoProject,
        InActiveProject,
        NoEmployee,
        NoQuestion,
        NotStarted,
        OnGoing,
        Completed,
        Halted
    }

    /// <summary>
    /// Daily Task Mail Email Sending Confirmation
    /// </summary>
    public enum SendEmailConfirmation
    {
        no,
        yes
    }

    /// <summary>
    /// Type of leaves. cl - casual leave and sl - sick leave
    /// </summary>
    public enum LeaveType
    {
        cl,
        sl
    }


    /// <summary>
    /// Scrum Answer Statuses
    /// </summary>
    public enum ScrumAnswerStatus
    {
        Answered,
        Leave
    }


    /// <summary>
    /// Scrum Actions
    /// </summary>
    public enum ScrumActions
    {
        halt,
        resume,
        start
    }

    /// <summary>
    /// Type of Bot
    /// </summary>
    public enum BotQuestionType
    {
        Scrum = 1,
        TaskMail = 2
    }

    /// <summary>
    /// Bot Question order 
    /// </summary>
    public enum QuestionOrder
    {
        YourTask = 1,
        HoursSpent = 2,
        Status = 3,
        Comment = 4,
        SendEmail = 5,
        ConfirmSendEmail = 6,
        TaskMailSend = 7,
        Yesterday = 8,
        Today = 9,
        RoadBlock = 10
    }

    public enum TaskMailCondition
    {
        Null = 1,
        NotNull = 2
    }
}
