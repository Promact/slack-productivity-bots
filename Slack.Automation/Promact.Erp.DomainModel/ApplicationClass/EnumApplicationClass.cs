using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        roadblock
    }

    /// <summary>
    /// Daily Task Mail Question Order
    /// </summary>
    public enum TaskMailQuestion
    {
        YourTask=1,
        HoursSpent=2,
        Status=3,
        Comment=4,
        SendEmail=5,
        ConfirmSendEmail=6,
        TaskMailSend=7
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
}
