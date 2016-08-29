using Autofac;
using Promact.Core.Repository.ScrumRepository;
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
        /// Method LeaveApply Testing with True Value
        /// </summary>
        [Fact, Trait("Category", "Required")]
        public async void ScrumInitiateNoQuestion()
        {

            //LeaveRequest leave = new LeaveRequest() { FromDate = DateTime.UtcNow, EndDate = DateTime.UtcNow, Reason = "testing", RejoinDate = DateTime.UtcNow, Status = Condition.Pending, Type = "Casual", CreatedOn = DateTime.UtcNow, EmployeeId = "fc172bd7-42d9-4cbf-baa8-130be02d25ed" };
            //_leaveRequestRepository.ApplyLeave(leave);
            //Assert.NotEqual(51, leave.Id);
            var msg = await _scrumBotRepository.StartScrum("trainees");
            Assert.Equal("Sorry I have nothing to ask you", msg);
            //  Assert.NotEqual(51, leave.Id);
        }
    }
}
