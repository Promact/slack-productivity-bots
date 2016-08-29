using Autofac;
using Moq;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.Util;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Promact.Core.Test
{
    public class SlackRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly IClient _client;
        private readonly ISlackRepository _slackRepository;
        private readonly Mock<IHttpClientRepository> _mockHttpClient;
        public SlackRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _client = _componentContext.Resolve<IClient>();
            _slackRepository = _componentContext.Resolve<ISlackRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
        }

        [Fact, Trait("Category", "Required")]
        public async void LeaveApply()
        {
            var httpResponse = new HttpResponseMessage();
            httpResponse.Content = new StringContent("");
            var response = Task.FromResult(httpResponse);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, userName);
            _mockHttpClient.Setup(x => x.GetAsync(AppSettingsUtil.UserUrl, requestUrl, accessToken)).Returns(response);
            SlashCommand leave = new SlashCommand() { Text = "Apply Hello 30-09-2016 30-09-2016 Casual 30-09-2016", Username = "siddhartha", ResponseUrl = AppSettingsUtil.IncomingWebHookUrl };
            var slackText = leave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            var leaveDetails = await _slackRepository.LeaveApply(slackText, leave, accessToken);
            Assert.Equal(leaveDetails.Status, Condition.Pending);
        }

        private string userName = "siddhartha";
        private string accessToken = "94d56876-fb02-45a9-8b01-c56046482d17";
        private User user = new User()
        {
            Id = "asjdfjasndlkmasdml41fgdf4g2df42",
            Email = "siddhartha@promactinfo.com",
            FirstName = "siddhartha",
            IsActive = true,
            LastName = "shaw",
            UserName = "siddhartha@promactinfo.com"
        };
        //[Fact]
        //public void LeaveListUpdateLeave()
        //{
        //    var leave = _slackRepository.UpdateLeave(19, "Rejected");
        //    Assert.Equal(leave.Status, Condition.Rejected);
        //}
    }
}
