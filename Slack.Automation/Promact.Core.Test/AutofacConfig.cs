using Autofac;
using Promact.Erp.DomainModel.Context;
using System.Data.Entity;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.OauthCallsRepository;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.Util.Email;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.ScrumRepository;
using Promact.Core.Repository.LeaveReportRepository;
using Promact.Erp.DomainModel.Models;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.TaskMailRepository;
using Effort;
using Promact.Core.Repository.BotQuestionRepository;
using Moq;
using Promact.Core.Repository.SlackChannelRepository;
using Promact.Core.Repository.ExternalLoginRepository;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Core.Repository.ScrumReportRepository;
using Promact.Erp.Util.EnvironmentVariableRepository;
using Promact.Core.Test.EnvironmentVariableRepository;
using Promact.Erp.Util.StringConstants;
using Promact.Core.Repository.EmailServiceTemplateRepository;
using Promact.Erp.Util.HttpClient;
using Autofac.Extras.NLog;
using AutoMapper;
using Promact.Core.Repository.AutoMapperConfig;
using Promact.Core.Repository.ServiceRepository;
using Promact.Core.Repository.BaseRepository;

namespace Promact.Core.Test
{
    public class AutofacConfig
    {
        public static IComponentContext RegisterDependancies()
        {
            var builder = new ContainerBuilder();
            var dataContext = new PromactErpContext(DbConnectionFactory.CreateTransient());
            builder.RegisterInstance(dataContext).As<DbContext>().SingleInstance();
            var httpClientMock = new Mock<IHttpClientService>();
            var httpClientMockObject = httpClientMock.Object;
            builder.RegisterInstance(httpClientMock).As<Mock<IHttpClientService>>();
            builder.RegisterInstance(httpClientMockObject).As<IHttpClientService>();
            builder.RegisterType<ApplicationUserStore>().As<IUserStore<ApplicationUser>>();
            builder.RegisterType<ApplicationUserManager>().AsSelf();
            builder.RegisterType<ApplicationSignInManager>().AsSelf();

            builder.RegisterType<EnvironmentVariableTestRepository>().As<IEnvironmentVariableRepository>();
            builder.Register<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication);
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            builder.RegisterType<LeaveRequestRepository>().As<ILeaveRequestRepository>();
            builder.RegisterType<ScrumBotRepository>().As<IScrumBotRepository>();
            builder.RegisterType<LeaveReportRepository>().As<ILeaveReportRepository>();
            builder.RegisterType<ScrumReportRepository>().As<IScrumReportRepository>();
            builder.RegisterType<OAuthLoginRepository>().As<IOAuthLoginRepository>();
            builder.RegisterType<Client>().As<IClient>();
            builder.RegisterType<OauthCallsRepository>().As<IOauthCallsRepository>();
            builder.RegisterType<SlackRepository>().As<ISlackRepository>();
            builder.RegisterType<AttachmentRepository>().As<IAttachmentRepository>();
            builder.RegisterType<SlackUserRepository>().As<ISlackUserRepository>();
            builder.RegisterType<StringConstantRepository>().As<IStringConstantRepository>();
            builder.RegisterType<SlackChannelRepository>().As<ISlackChannelRepository>();
            builder.RegisterType<TaskMailRepository>().As<ITaskMailRepository>();
            builder.RegisterType<BotQuestionRepository>().As<IBotQuestionRepository>();
            builder.RegisterType<ServiceRepository>().As<IServiceRepository>();
            var emailServiceMock = new Mock<IEmailService>();
            var emailServiceMockObject = emailServiceMock.Object;
            builder.RegisterInstance(emailServiceMock).As<Mock<IEmailService>>();
            builder.RegisterInstance(emailServiceMockObject).As<IEmailService>();
            builder.RegisterType<EmailServiceTemplateRepository>().As<IEmailServiceTemplateRepository>();
            var iLoggerMock = new Mock<ILogger>();
            var iLoggerMockObject = iLoggerMock.Object;
            builder.RegisterInstance(iLoggerMock).As<Mock<ILogger>>();
            builder.RegisterInstance(iLoggerMockObject).As<ILogger>();
            builder.Register(x => AutoMapperConfiguration.ConfigureMap()).As<IMapper>().SingleInstance();

            var mockServiceRepository = new Mock<IServiceRepository>();
            var mockServiceRepositoryObject = mockServiceRepository.Object;
            builder.RegisterInstance(mockServiceRepository).As<Mock<IServiceRepository>>();
            builder.RegisterInstance(mockServiceRepositoryObject).As<IServiceRepository>();
            
            var container = builder.Build();
            return container;

        }
    }
}
