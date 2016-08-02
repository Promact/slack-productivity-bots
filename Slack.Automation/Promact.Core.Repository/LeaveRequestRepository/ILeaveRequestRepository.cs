using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.LeaveRequestRepository
{
    public interface ILeaveRequestRepository
    {
        void ApplyLeave(LeaveRequest leave);
        IEnumerable<LeaveRequest> LeaveList();
        LeaveRequest CancelLeave(int leaveId);
        IEnumerable<LeaveRequest> LeaveListByUserId(string userId);
        LeaveRequest LeaveListStatusByUserId(string userId);
        LeaveRequest LeaveById(int leaveId);
        void UpdateLeave(LeaveRequest leave);
    }
}
