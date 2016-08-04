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

        /// <summary>
        /// Method to apply Leave
        /// </summary>
        /// <param name="leave"></param>
        public void ApplyLeave(LeaveRequest leave)
        {
            _leaveRequestRepository.Insert(leave);
            _leaveRequestRepository.Save();
        }

        /// <summary>
        /// Method to get All List of leave
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LeaveRequest> LeaveList()
        {
            return _leaveRequestRepository.List();
        }

        /// <summary>
        /// Method used to cancel the leave request using its integer leaveId
        /// </summary>
        /// <param name="leaveId"></param>
        /// <returns></returns>
        public LeaveRequest CancelLeave(int leaveId)
        {
            var leave = _leaveRequestRepository.GetById(leaveId);
            leave.Status = Condition.Cancel;
            _leaveRequestRepository.Update(leave);
            _leaveRequestRepository.Save();
            return leave;
        }

        /// <summary>
        /// Method to get leave list corresponding each user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<LeaveRequest> LeaveListByUserId(string userId)
        {
            return _leaveRequestRepository.Fetch(x => x.EmployeeId == userId);
        }

        /// <summary>
        /// Method to get the last leave request status corresponding to each user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get a particular leave detail using leaveId
        /// </summary>
        /// <param name="leaveId"></param>
        /// <returns></returns>
        public LeaveRequest LeaveById(int leaveId)
        {
            var leave = _leaveRequestRepository.FirstOrDefault(x => x.Id == leaveId);
            return leave;
        }

        /// <summary>
        /// Method to update leave request
        /// </summary>
        /// <param name="leave"></param>
        public void UpdateLeave(LeaveRequest leave)
        {
            _leaveRequestRepository.Update(leave);
            _leaveRequestRepository.Save();
        }
    }
}
