using Autofac;
using Moq;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using Promact.Erp.DomainModel.Models;
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
        private readonly Mock<IClient> _mockClient;
        private readonly IAttachmentRepository _attachmentRepository;
        public SlackRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _client = _componentContext.Resolve<IClient>();
            _slackRepository = _componentContext.Resolve<ISlackRepository>();
            _mockHttpClient = _componentContext.Resolve<Mock<IHttpClientRepository>>();
            _mockClient = _componentContext.Resolve<Mock<IClient>>();
            _attachmentRepository = _componentContext.Resolve<IAttachmentRepository>();
        }

        /// <summary>
        /// Test cases for checking method LeaveApply from Slack respository
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void LeaveApply()
        {
            var response = Task.FromResult(StringConstant.UserDetailsFromOauthServer);
            var requestUrl = string.Format("{0}{1}", StringConstant.UserDetailsUrl, StringConstant.FirstNameForTest);
            _mockHttpClient.Setup(x => x.GetAsync(StringConstant.ProjectUserUrl, requestUrl, StringConstant.AccessTokenForTest)).Returns(response);
            SlashCommand leave = new SlashCommand() { Text = StringConstant.SlashCommandText, Username = StringConstant.FirstNameForTest, ResponseUrl = Environment.GetEnvironmentVariable(StringConstant.IncomingWebHookUrl, EnvironmentVariableTarget.User) };
            var slackText = leave.Text.Split('"')
                            .Select((element, index) => index % 2 == 0 ? element
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { element })
                            .SelectMany(element => element).ToList();
            LeaveRequest leaveRequest = new LeaveRequest();
            leaveRequest.Reason = StringConstant.Reason;
            var replyText = _attachmentRepository.ReplyText(StringConstant.FirstNameForTest, leaveRequest);
            _mockClient.Setup(x => x.SendMessage(leave, replyText));
            var leaveDetails = await _slackRepository.LeaveApply(slackText, leave, StringConstant.AccessTokenForTest);
            Assert.Equal(leaveDetails.Status, Condition.Pending);
        }

        private User user = new User()
        {
            Id = StringConstant.StringIdForTest,
            Email = StringConstant.EmailForTest,
            FirstName = StringConstant.FirstNameForTest,
            IsActive = true,
            LastName = StringConstant.LastNameForTest,
            UserName = StringConstant.EmailForTest
        };
    }
}
