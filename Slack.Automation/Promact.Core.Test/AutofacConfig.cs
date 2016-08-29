using Autofac;
using Promact.Erp.DomainModel.Context;
using System.Data.Entity;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.Util.Email;
using Promact.Core.Repository.AttachmentRepository;
using System.Net.Http;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.ScrumRepository;
using Promact.Core.Repository.LeaveReportRepository;
using Promact.Erp.DomainModel.Models;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Promact.Core.Repository.SlackUserRepository;
using Promact.Core.Repository.TaskMailRepository;

namespace Promact.Core.Test
{
    public class AutofacConfig
    {     
        public static IComponentContext RegisterDependancies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<PromactErpContext>().As<DbContext>();
            builder.RegisterType<ApplicationUserStore>().As<IUserStore<ApplicationUser>>();
            builder.RegisterType<ApplicationUserManager>().AsSelf();
            builder.RegisterType<ApplicationSignInManager>().AsSelf();
            builder.Register<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication);
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            builder.RegisterType<LeaveRequestRepository>().As<ILeaveRequestRepository>();
            builder.RegisterType<ScrumBotRepository>().As<IScrumBotRepository>();
            builder.RegisterType<LeaveReportRepository>().As<ILeaveReportRepository>();
            builder.RegisterType<Client>().As<IClient>();
            builder.RegisterType<ProjectUserCallRepository>().As<IProjectUserCallRepository>();
            builder.RegisterType<SlackRepository>().As<ISlackRepository>();
            builder.RegisterType<Promact.Erp.Util.Email.EmailService>().As<IEmailService>();
            builder.RegisterType<AttachmentRepository>().As<IAttachmentRepository>();
            builder.RegisterType<HttpClient>();
            builder.RegisterType<HttpClientRepository>().As<IHttpClientRepository>();
            builder.RegisterType<SlackUserRepository>().As<ISlackUserRepository>();
            builder.RegisterType<TaskMailRepository>().As<ITaskMailRepository>();
            var container = builder.Build();
            return container;
        }
    }
}
