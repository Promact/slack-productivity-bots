using Autofac;
using Promact.Core.Repository.ScrumRepository;
using Promact.Erp.Util;
using Xunit;

namespace Promact.Core.Test
{
    public class ScrumBotRepositoryTest
    {
        private readonly IComponentContext _componentContext;
        private readonly IScrumBotRepository _scrumBotRepository;

        public ScrumBotRepositoryTest()
        {
            _componentContext = AutofacConfig.RegisterDependancies();
            _scrumBotRepository = _componentContext.Resolve<IScrumBotRepository>();
        }

        /// <summary>
        /// Method StartScrum Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoQuestion()
        {
            //LeaveRequest leave = new LeaveRequest() { FromDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Reason = "testing", RejoinDate = DateTime.UtcNow, Status = Condition.Pending, Type = "Casual", CreatedOn = DateTime.UtcNow, EmployeeId = "fc172bd7-42d9-4cbf-baa8-130be02d25ed" };
            //_leaveRequestRepository.ApplyLeave(leave);
            //Assert.NotEqual(51, leave.Id);
            var msg = await _scrumBotRepository.StartScrum(StringConstant.GroupName, StringConstant.UserName);
            Assert.NotEqual(string.Empty, msg);
        }

        /// <summary>
        /// Method AddScrumAnswer Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void AddScrumAnswer()
        {
            var msg = await _scrumBotRepository.AddScrumAnswer(StringConstant.UserName, StringConstant.AnswerStatement, StringConstant.GroupName);
            Assert.NotEqual(string.Empty, msg);
        }

        /// <summary>
        /// Method Leave Testing with False Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void Leave()
        {
            var msg = await _scrumBotRepository.Leave(StringConstant.GroupName, StringConstant.UserName, StringConstant.LeaveApplicant);
            Assert.NotEqual(string.Empty, msg);
        }
    }
}