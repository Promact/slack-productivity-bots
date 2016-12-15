using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Promact.Core.Repository.LeaveRequestRepository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        #region Private Variable
        private readonly IRepository<LeaveRequest> _leaveRequestRepository;
        #endregion

        #region Constructor
        public LeaveRequestRepository(IRepository<LeaveRequest> leaveRequestRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to apply Leave
        /// </summary>
        /// <param name="leave"></param>
        public async Task ApplyLeaveAsync(LeaveRequest leave)
        {
            _leaveRequestRepository.Insert(leave);
            await _leaveRequestRepository.SaveChangesAsync();
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
        public async Task<LeaveRequest> CancelLeaveAsync(int leaveId)
        {
            var leave = _leaveRequestRepository.GetById(leaveId);
            leave.Status = Condition.Cancel;
            _leaveRequestRepository.Update(leave);
            await _leaveRequestRepository.SaveChangesAsync();
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
            var leaves = _leaveRequestRepository.Fetch(x => x.EmployeeId == userId);
            if (leaves.Count() != 0)
            {
                var leave = leaves.Last();
                return leave;
            }
            else
                throw new LeaveNotFoundForUser();
        }

        /// <summary>
        /// Get a particular leave detail using leaveId
        /// </summary>
        /// <param name="leaveId"></param>
        /// <returns>leave</returns>
        public async Task<LeaveRequest> LeaveByIdAsync(int leaveId)
        {
            var leave = await _leaveRequestRepository.FirstOrDefaultAsync(x => x.Id == leaveId);
            return leave;
        }

        /// <summary>
        /// Method to update leave request
        /// </summary>
        /// <param name="leave"></param>
        public async Task UpdateLeaveAsync(LeaveRequest leave)
        {
            _leaveRequestRepository.Update(leave);
            await _leaveRequestRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Method to get number of leave taken by a user
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>number of leave taken</returns>
        public LeaveAllowed NumberOfLeaveTaken(string employeeId)
        {
            double casualLeaveTaken = 0.0;
            double sickLeaveTaken = 0.0;
            var leaveList = _leaveRequestRepository.Fetch(x => x.EmployeeId == employeeId && x.Status == Condition.Approved);
            foreach (var leave in leaveList)
            {
                var leaveTaken = leave.EndDate.Value.Day - leave.FromDate.Day +1;
                if (leave.Type == LeaveType.cl)
                {
                    casualLeaveTaken += Convert.ToDouble(leaveTaken);
                }
                else
                {
                    sickLeaveTaken += Convert.ToDouble(leaveTaken);
                }
            }
            LeaveAllowed leaveAllowed = new LeaveAllowed();
            leaveAllowed.CasualLeave = casualLeaveTaken;
            leaveAllowed.SickLeave = sickLeaveTaken;
            return leaveAllowed;
        }

        /// <summary>
        /// Method to get leave list corresponding each user, only approved and pending status
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of leave of a particular user</returns>
        public IEnumerable<LeaveRequest> LeaveListByUserIdOnlyApprovedAndPending(string userId)
        {
            return _leaveRequestRepository.Fetch(x => x.EmployeeId == userId && x.Status == Condition.Approved || x.Status == Condition.Pending);
        }
        #endregion
    }
}
