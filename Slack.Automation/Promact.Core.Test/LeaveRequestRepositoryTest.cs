using Autofac;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace Promact.Core.Test
{
    /// <summary>
    /// Test Cases of Leave Request Repository
    /// </summary>
    public class LeaveRequestRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        public LeaveRequestRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _leaveRequestRepository = _componentContext.Resolve<ILeaveRequestRepository>();
        }

        /// <summary>
        /// Method LeaveApply Testing with True Value
        /// </summary>
        [Fact]
        public void ApplyLeave()
        {
            LeaveRequest leave = new LeaveRequest() { FromDate=DateTime.UtcNow, EndDate = DateTime.UtcNow, Reason="testing", RejoinDate=DateTime.UtcNow, Status= Condition.Pending, Type="Casual", CreatedOn = DateTime.UtcNow, EmployeeId= "fc172bd7-42d9-4cbf-baa8-130be02d25ed" };
            _leaveRequestRepository.ApplyLeave(leave);
            Assert.NotEqual(51, leave.Id);
        }

        /// <summary>
        /// Method LeaveList Testing with True Value
        /// </summary>
        [Fact]
        public void LeaveList()
        {
            var leaves = _leaveRequestRepository.LeaveList();
            Assert.Equal(68, leaves.Count());
        }

        /// <summary>
        /// Method CancelLeave Testing with True Value
        /// </summary>
        [Fact]
        public void CancelLeave()
        {
            var leaves = _leaveRequestRepository.CancelLeave(50);
            Assert.Equal(Condition.Cancel, leaves.Status);
        }

        /// <summary>
        /// Method LeaveListByUserId Testing with True Value
        /// </summary>
        [Fact]
        public void LeaveListByUserId()
        {
            int status = 0;
            var leaves = _leaveRequestRepository.LeaveListByUserId("c8735a71-619f-47b7-9da2-6761d1cc1972");
            foreach (var leave in leaves)
            {
                status = leave.Id;
            }
            Assert.Equal(50, status);
        }

        /// <summary>
        /// Method LeaveListStatusByUserId Testing with True Value
        /// </summary>
        [Fact]
        public void LeaveListStatusByUserId()
        {
            var leaves = _leaveRequestRepository.LeaveListStatusByUserId("c8735a71-619f-47b7-9da2-6761d1cc1972");
            Assert.Equal(Condition.Cancel, leaves.Status);
        }

        /// <summary>
        /// Method LeaveById Testing with True Value
        /// </summary>
        [Fact]
        public void LeaveById()
        {
            var leaves = _leaveRequestRepository.LeaveById(30);
            Assert.Equal(Condition.Pending, leaves.Status);
        }

        /// <summary>
        /// Method UpdateLeave Testing with True Value
        /// </summary>
        [Fact]
        public void UpdateLeave()
        {
            var leaves = _leaveRequestRepository.LeaveById(40);
            leaves.Status = Condition.Rejected;
            _leaveRequestRepository.UpdateLeave(leaves);
            Assert.Equal(Condition.Rejected, leaves.Status);
        }


        /// <summary>
        /// Method ApplyLeave Testing with False Value
        /// </summary>
        [Fact]
        public void ApplyLeaveFalse()
        {
            LeaveRequest leave = new LeaveRequest() { FromDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Reason = "testing", RejoinDate = DateTime.UtcNow, Status = Condition.Pending, Type = "Casual", CreatedOn = DateTime.UtcNow, EmployeeId = "fc172bd7-42d9-4cbf-baa8-130be02d25ed" };
            _leaveRequestRepository.ApplyLeave(leave);
            Assert.NotEqual(31, leave.Id);
        }

        /// <summary>
        /// Method LeaveList Testing with False Value
        /// </summary>
        [Fact]
        public void LeaveListFalse()
        {
            var leaves = _leaveRequestRepository.LeaveList();
            Assert.NotEqual(20, leaves.Count());
        }

        /// <summary>
        /// Method CancelLeave Testing with False Value
        /// </summary>
        [Fact]
        public void CancelLeaveFalse()
        {
            var leaves = _leaveRequestRepository.CancelLeave(25);
            Assert.NotEqual(Condition.Pending, leaves.Status);
        }

        /// <summary>
        /// Method LeaveListByUserId Testing with False Value
        /// </summary>
        [Fact]
        public void LeaveListByUserIdFalse()
        {
            int Id = 0;
            var leaves = _leaveRequestRepository.LeaveListByUserId("c8735a71-619f-47b7-9da2-6761d1cc1972");
            foreach (var leave in leaves)
            {
                Id = leave.Id;
            }
            Assert.NotEqual(100, Id);
        }

        /// <summary>
        /// Method LeaveListStatusByUserId Testing with False Value
        /// </summary>
        [Fact]
        public void LeaveListStatusByUserIdFalse()
        {
            var leaves = _leaveRequestRepository.LeaveListStatusByUserId("c8735a71-619f-47b7-9da2-6761d1cc1972");
            Assert.NotEqual(Condition.Approved, leaves.Status);
        }

        /// <summary>
        /// Method LeaveById Testing with False Value
        /// </summary>
        [Fact]
        public void LeaveByIdFalse()
        {
            var leaves = _leaveRequestRepository.LeaveById(30);
            Assert.NotEqual(Condition.Approved, leaves.Status);
        }

        /// <summary>
        /// Method UpdateLeave Testing with False Value
        /// </summary>
        [Fact]
        public void UpdateLeaveFalse()
        {
            var leaves = _leaveRequestRepository.LeaveById(40);
            leaves.Status = Condition.Rejected;
            _leaveRequestRepository.UpdateLeave(leaves);
            Assert.NotEqual(Condition.Approved, leaves.Status);
        }
    }
}
