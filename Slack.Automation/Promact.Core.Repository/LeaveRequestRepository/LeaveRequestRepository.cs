using Promact.Core.Repository.DataRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.LeaveRequestRepository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;
        public LeaveRequestRepository(IRepository<LeaveRequest> leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }
        public void ApplyLeave(LeaveRequest leave)
        {
            _leaveRequestRepository.Insert(leave);
            _leaveRequestRepository.Save();
        }
        public IEnumerable<LeaveRequest> LeaveList()
        {
            return _leaveRequestRepository.List();
        }
        public LeaveRequest CancelLeave(int leaveId)
        {
            var leave = _leaveRequestRepository.GetById(leaveId);
            leave.Status = Condition.Cancel;
            _leaveRequestRepository.Update(leave);
            _leaveRequestRepository.Save();
            return leave;
        }
        public IEnumerable<LeaveRequest> LeaveListByUserId(string userId)
        {
            return _leaveRequestRepository.Fetch(x => x.EmployeeId == userId);
        }
        public LeaveRequest LeaveListStatusByUserId(string userId)
        {
            var leaveList = _leaveRequestRepository.Fetch(x => x.EmployeeId == userId);
            LeaveRequest leave = new LeaveRequest();
            foreach (var leaves in leaveList)
            {
                leave = leaves;
            }
            return leave;
        }
        public LeaveRequest LeaveById(int leaveId)
        {
            var leave = _leaveRequestRepository.FirstOrDefault(x => x.Id == leaveId);
            return leave;
        }
        public void UpdateLeave(LeaveRequest leave)
        {
            _leaveRequestRepository.Update(leave);
            _leaveRequestRepository.Save();
        }
    }
}
