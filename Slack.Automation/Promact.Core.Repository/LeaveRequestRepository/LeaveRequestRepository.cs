using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <returns>List of leave</returns>
        public IEnumerable<LeaveRequest> LeaveList()
        {
            return _leaveRequestRepository.GetAll();
        }

        /// <summary>
        /// Method used to cancel the leave request using its integer leaveId
        /// </summary>
        /// <param name="leaveId"></param>
        /// <returns>leave which has been cancelled</returns>
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
        /// <returns>List of leave of a particular user</returns>
        public IEnumerable<LeaveRequest> LeaveListByUserId(string userId)
        {
            return _leaveRequestRepository.Fetch(x => x.EmployeeId == userId);
        }

        /// <summary>
        /// Method to get the last leave request status corresponding to each user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>latest leave details of a particular user</returns>
        public LeaveRequest LeaveListStatusByUserId(string userId)
        {
            var leave = _leaveRequestRepository.Fetch(x => x.EmployeeId == userId).Last();
            return leave;
        }

        /// <summary>
        /// Get a particular leave detail using leaveId
        /// </summary>
        /// <param name="leaveId"></param>
        /// <returns>leave</returns>
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

        /// <summary>
        /// Method to get number of leave taken by a user
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>number of leave taken</returns>
        public double NumberOfLeaveTaken(string employeeId)
        {
            double casualLeaveTaken = 0.0;
            var leaveList = _leaveRequestRepository.Fetch(x => x.EmployeeId == employeeId && x.Status == Condition.Approved);
            foreach (var leave in leaveList)
            {
                var leaveTaken = leave.EndDate.Value.Day - leave.FromDate.Day;
                casualLeaveTaken += Convert.ToDouble(leaveTaken);
            }
            return casualLeaveTaken;
        }
    }
}
