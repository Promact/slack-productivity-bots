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
        Inprogress,
        Completed,
        Roadblock
    }
}
