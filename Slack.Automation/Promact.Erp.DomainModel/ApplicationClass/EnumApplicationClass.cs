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

    public enum SlackAction
    {
        Apply,
        List,
        Cancel,
        Status,
        Balance,
        Help
    }

    public enum TaskMailStatus
    {
        inprogress,
        completed,
        roadblock
    }
    public enum TaskMailQuestion
    {
        YourTask=1,
        HoursSpent=2,
        Status=3,
        Comment=4,
        SendEmail=5
    }
}
