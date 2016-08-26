using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Promact.Erp.DomainModel.ApplicationClass;

namespace Promact.Core.Repository.LeaveReportRepository
{
    public interface ILeaveReportRepository
    {
        Task<IEnumerable<LeaveReport>> LeaveReport(string accessToken);

        Task <IEnumerable<LeaveReportDetails>> LeaveReportDetails(string employeeId, string accessToken);
        
    }
}
