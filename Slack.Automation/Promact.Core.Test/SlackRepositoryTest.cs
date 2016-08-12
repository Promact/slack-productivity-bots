using Autofac;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.Util;
using System;
using System.Linq;
using Xunit;

namespace Promact.Core.Test
{
    public class SlackRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly IClient _client;
        private readonly ISlackRepository _slackRepository;
        public SlackRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _client = _componentContext.Resolve<IClient>();
            _slackRepository = _componentContext.Resolve<ISlackRepository>();
        }

        [Fact, Trait("Category", "Required")]
        public async void LeaveApply()
        {
            SlashCommand leave = new SlashCommand() { Text = "Apply Hello 30-09-2016 30-09-2016 Casual 30-09-2016", Username="siddhartha", ResponseUrl=AppSettingsUtil.IncomingWebHookUrl };
            var slackText = leave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var leaveDetails = await _slackRepository.LeaveApply(slackText, leave);
            Assert.Equal(leaveDetails.Status,Condition.Pending);
        }

        //[Fact]
        //public void LeaveListUpdateLeave()
        //{
        //    var leave = _slackRepository.UpdateLeave(19, "Rejected");
        //    Assert.Equal(leave.Status, Condition.Rejected);
        //}
    }
}
