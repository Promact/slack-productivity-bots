using Autofac;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class SlackRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly ISlackRepository _slackRepository;
        public SlackRepositoryTest(ISlackRepository slackRepository)
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _slackRepository = _componentContext.Resolve<ISlackRepository>();
        }

        /// <summary>
        /// Testing with True Value
        /// </summary>
        [Fact]
        public async void LeaveApply()
        {
            SlashCommand leave = new SlashCommand() { Text = "apply Hello 02-08-2016 03-08-2016 Casual 04-08-2016" };
            var slackText = leave.Text.Split('"').Select((element, index) => index % 2 == 0 ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element }).SelectMany(element => element).ToList();
            var leaveDetails = await _slackRepository.LeaveApply(slackText, "siddhartha");
            Assert.Equal(leaveDetails.Status, Condition.Pending);
        }

        [Fact]
        public async void LeaveList()
        {
            var leaveDetails = ' ';
            var leaves = await _slackRepository.LeaveList("siddhartha");
            foreach (var leave in leaves)
            {
                leaveDetails = leave;
            }
            Assert.NotEqual(leaveDetails, ' ');
        }

        [Fact]
        public async void CancelLeave()
        {
            var leaves = await _slackRepository.CancelLeave(1, "siddhartha");
            Assert.NotEqual(leaves, "");
        }

        [Fact]
        public async void LeaveStatus()
        {
            var leaves = await _slackRepository.LeaveStatus("siddhartha");
            Assert.NotEqual(leaves, "");
        }

        [Fact]
        public void UpdateLeave()
        {
            var leaves = _slackRepository.UpdateLeave(28,"Rejected");
            Assert.Equal(leaves.Status, Condition.Rejected);
        }

        /// <summary>
        /// Testing with False Value
        /// </summary>
        [Fact]
        public async void LeaveApplyFalse()
        {
            SlashCommand leave = new SlashCommand() { Text = "apply Hello 02-08-2016 03-08-2016 Casual 04-08-2016" };
            var slackText = leave.Text.Split('"').Select((element, index) => index % 2 == 0 ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element }).SelectMany(element => element).ToList();
            var leaveDetails = await _slackRepository.LeaveApply(slackText, "siddhartha");
            Assert.NotEqual(leaveDetails.Status, Condition.Approved);
        }

        [Fact]
        public void UpdateLeaveFalse()
        {
            var leaves = _slackRepository.UpdateLeave(36, "Rejected");
            Assert.NotEqual(leaves.Status, Condition.Approved);
        }
    }
}
