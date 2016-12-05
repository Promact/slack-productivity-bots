using Autofac;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util;
using Promact.Erp.Util.StringConstants;
using System;
using System.Linq;
using Xunit;


namespace Promact.Core.Test
{
    /// <summary>
    /// Test Cases of Leave Request Repository
    /// </summary>
    public class LeaveRequestRepositoryTest
    {
        #region Private Varaibles
        private readonly IComponentContext _componentContext;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IStringConstantRepository _stringConstant;
        private LeaveRequest leave = new LeaveRequest();
        #endregion

        #region Constructor
        public LeaveRequestRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _leaveRequestRepository = _componentContext.Resolve<ILeaveRequestRepository>();
            _stringConstant = _componentContext.Resolve<IStringConstantRepository>();
            Initialize();
        }
        #endregion

        #region Test Cases
        /// <summary>
        /// Method LeaveApply Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void ApplyLeave()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            Assert.Equal(1, leave.Id);
        }

        /// <summary>
        /// Method LeaveList Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void LeaveList()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var leaves = _leaveRequestRepository.LeaveList();
            Assert.Equal(3, leaves.Count());
        }

        /// <summary>
        /// Method CancelLeave Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void CancelLeave()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            var leaves = _leaveRequestRepository.CancelLeave(1);
            Assert.Equal(Condition.Cancel, leaves.Status);
        }

        /// <summary>
        /// Method LeaveListByUserId Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void LeaveListByUserId()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var status = new Condition();
            var leaves = _leaveRequestRepository.LeaveListByUserId(_stringConstant.StringIdForTest);
            foreach (var leave in leaves)
            {
                status = leave.Status;
            }
            Assert.Equal(Condition.Pending, status);
        }

        /// <summary>
        /// Method LeaveListStatusByUserId Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void LeaveListStatusByUserId()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            var leaves = _leaveRequestRepository.LeaveListStatusByUserId(_stringConstant.StringIdForTest);
            Assert.NotEqual(Condition.Cancel, leaves.Status);
        }

        /// <summary>
        /// Method LeaveById Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void LeaveById()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            var leaves = _leaveRequestRepository.LeaveById(1);
            Assert.Equal(Condition.Pending, leaves.Status);
        }

        /// <summary>
        /// Method UpdateLeave Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void UpdateLeave()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            var leaves = _leaveRequestRepository.LeaveById(1);
            leaves.Status = Condition.Rejected;
            _leaveRequestRepository.UpdateLeave(leaves);
            Assert.Equal(Condition.Rejected, leaves.Status);
        }


        /// <summary>
        /// Method ApplyLeave Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void ApplyLeaveFalse()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            Assert.NotEqual(31, leave.Id);
        }

        /// <summary>
        /// Method LeaveList Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void LeaveListFalse()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            var leaves = _leaveRequestRepository.LeaveList();
            Assert.NotEqual(20, leaves.Count());
        }

        /// <summary>
        /// Method CancelLeave Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void CancelLeaveFalse()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            _leaveRequestRepository.ApplyLeave(leave);
            var leaves = _leaveRequestRepository.CancelLeave(2);
            Assert.NotEqual(Condition.Pending, leaves.Status);
        }

        /// <summary>
        /// Method LeaveListByUserId Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void LeaveListByUserIdFalse()
        {
            int Id = 0;
            _leaveRequestRepository.ApplyLeave(leave);
            var leaves = _leaveRequestRepository.LeaveListByUserId(_stringConstant.StringIdForTest);
            foreach (var leave in leaves)
            {
                Id = leave.Id;
            }
            Assert.NotEqual(100, Id);
        }

        /// <summary>
        /// Method LeaveListStatusByUserId Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void LeaveListStatusByUserIdFalse()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            var leaves = _leaveRequestRepository.LeaveListStatusByUserId(_stringConstant.StringIdForTest);
            Assert.NotEqual(Condition.Approved, leaves.Status);
        }

        /// <summary>
        /// Method LeaveById Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void LeaveByIdFalse()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            var leaves = _leaveRequestRepository.LeaveById(1);
            Assert.NotEqual(Condition.Approved, leaves.Status);
        }

        /// <summary>
        /// Method UpdateLeave Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void UpdateLeaveFalse()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            var leaves = _leaveRequestRepository.LeaveById(1);
            leaves.Status = Condition.Rejected;
            _leaveRequestRepository.UpdateLeave(leaves);
            Assert.NotEqual(Condition.Approved, leaves.Status);
        }

        /// <summary>
        /// Method NumberOfLeaveTaken testing with True Value Casual Leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void NumberOfLeaveTakenCasual()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            leave.Status = Condition.Approved;
            _leaveRequestRepository.UpdateLeave(leave);
            var casualLeave = _leaveRequestRepository.NumberOfLeaveTaken(leave.EmployeeId);
            Assert.NotEqual(1,casualLeave.CasualLeave);
        }


        /// <summary>
        /// Method NumberOfLeaveTaken testing with false Value Casual Leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void NumberOfLeaveTakenFalseCasual()
        {
            _leaveRequestRepository.ApplyLeave(leave);
            var casualLeave = _leaveRequestRepository.NumberOfLeaveTaken(_stringConstant.SlackChannelIdForTest);
            Assert.Equal(0.0, casualLeave.CasualLeave);
        }

        /// <summary>
        /// Method NumberOfLeaveTaken testing with True Value Casual Leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void NumberOfLeaveTakenSick()
        {
            leave.Type = LeaveType.sl;
            _leaveRequestRepository.ApplyLeave(leave);
            leave.Status = Condition.Approved;
            _leaveRequestRepository.UpdateLeave(leave);
            var casualLeave = _leaveRequestRepository.NumberOfLeaveTaken(leave.EmployeeId);
            Assert.NotEqual(1, casualLeave.CasualLeave);
        }


        /// <summary>
        /// Method NumberOfLeaveTaken testing with false Value Casual Leave
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public void NumberOfLeaveTakenFalseSick()
        {
            leave.Type = LeaveType.sl;
            _leaveRequestRepository.ApplyLeave(leave);
            var casualLeave = _leaveRequestRepository.NumberOfLeaveTaken(_stringConstant.SlackChannelIdForTest);
            Assert.Equal(0.0, casualLeave.CasualLeave);
        }
        #endregion

        #region Initialisation
        /// <summary>
        /// A method is used to initialize variables which are repetitively used
        /// </summary>
        public void Initialize()
        {
            leave.FromDate = DateTime.UtcNow;
            leave.EndDate = DateTime.UtcNow;
            leave.Reason = _stringConstant.LeaveReasonForTest;
            leave.RejoinDate = DateTime.UtcNow;
            leave.Status = Condition.Pending;
            leave.Type = LeaveType.cl;
            leave.CreatedOn = DateTime.UtcNow;
            leave.EmployeeId = _stringConstant.StringIdForTest;
        }
        #endregion
    }
}
