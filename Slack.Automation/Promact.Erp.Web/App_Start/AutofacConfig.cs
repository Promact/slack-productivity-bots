using Autofac;
using Autofac.Extras.NLog;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.BotQuestionRepository;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.ExternalLoginRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.LeaveReportRepository;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Core.Repository.ScrumRepository;
using Promact.Core.Repository.SlackChannelRepository;
using Promact.Core.Repository.SlackRepository;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.TaskMailRepository;
using Promact.Erp.Core.Controllers;
using Promact.Erp.DomainModel.Context;
using Promact.Erp.DomainModel.DataRepository;
using Promact.Erp.DomainModel.Models;
using Promact.Erp.Util.Email;
using System.Data.Entity;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Promact.Erp.Web.App_Start
{
    public static class AutofacConfig
    {
        public static IComponentContext RegisterDependancies()
        {
            var builder = new ContainerBuilder();
            // register dependency
            builder.RegisterType<PromactErpContext>().As<DbContext>();
            builder.RegisterType<ApplicationUserStore>().As<IUserStore<ApplicationUser>>();
            builder.RegisterType<ApplicationUserManager>().AsSelf();
            builder.RegisterType<ApplicationSignInManager>().AsSelf();
            builder.Register<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication);
            // register webapi controller
            builder.RegisterApiControllers(typeof(OAuthController).Assembly);

            // register mvc controller
            builder.RegisterControllers(typeof(HomeController).Assembly);

            // register webapi controller
            builder.RegisterApiControllers(typeof(LeaveRequestController).Assembly);
            builder.RegisterApiControllers(typeof(LeaveReportController).Assembly);

            // register repositories
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            builder.RegisterType<LeaveRequestRepository>().As<ILeaveRequestRepository>();
            builder.RegisterType<SlackRepository>().As<ISlackRepository>();
            builder.RegisterType<ScrumBotRepository>().As<IScrumBotRepository>();
            builder.RegisterType<Client>().As<IClient>();
            builder.RegisterType<ProjectUserCallRepository>().As<IProjectUserCallRepository>();
            builder.RegisterType<Util.Email.EmailService>().As<IEmailService>();
            builder.RegisterType<AttachmentRepository>().As<IAttachmentRepository>();
            builder.RegisterType<HttpClient>().InstancePerDependency();
            builder.RegisterType<HttpClientRepository>().As<IHttpClientRepository>();
            builder.RegisterType<LeaveReportRepository>().As<ILeaveReportRepository>();
            builder.RegisterType<TaskMailRepository>().As<ITaskMailRepository>();
            builder.RegisterType<SlackUserRepository>().As<ISlackUserRepository>();
            builder.RegisterType<BotQuestionRepository>().As<IBotQuestionRepository>();
            builder.RegisterType<OAuthLoginRepository>().As<IOAuthLoginRepository>();
            builder.RegisterType<SlackChannelRepository>().As<ISlackChannelRepository>();
            builder.RegisterModule<NLogModule>();
            builder.RegisterModule<SimpleNLogModule>();
            var container = builder.Build();

            // replace mvc dependancy resolver with autofac
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // replace webapi dependancy resolver with autofac
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            return container;
        }
    }
}