
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
        update,
        projects,
        issues,
        apikey
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
        RoadBlock = 10,
        RestartTask = 11
    }

    public enum TaskMailCondition
    {
        Null = 1,
        NotNull = 2
    }

    public enum Priority
    {
        Low = 3,
        Normal = 4,
        High = 5,
        Urgent = 6,
        Immediate = 7
    }

    public enum Status
    {
        New = 1,
        InProgess = 2,
        Confirmed = 3,
        Resolved = 4,
        Hold = 5,
        Feedback = 6,
        Closed = 7,
        Rejected = 8
    }

    public enum Tracker
    {
        Bug = 1,
        Feature = 2,
        Support = 3,
        Tasks = 4
    }

    public enum RedmineAction
    {
        list,
        create,
        changeassignee,
        close,
        timeentry
    }

    public enum TimeEntryActivity
    {
        Analysis = 7,
        Design = 8,
        Development = 9,
        Testing = 10,
        Roadblock = 11
    }
}
