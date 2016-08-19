using Autofac;
using Autofac.Integration.WebApi;
using Promact.Core.Repository.AttachmentRepository;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.HttpClientRepository;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Core.Repository.ScrumRepository;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.Core.Controllers;
using Promact.Erp.DomainModel.Context;
using Promact.Erp.Util.Email;
using System.Data.Entity;
using System.Net.Http;
using System.Web.Http;

namespace Promact.Erp.Web.App_Start
{
    public static class AutofacConfig
    {
        public static IComponentContext RegisterDependancies()
        {
            var builder = new ContainerBuilder();
            // register dependency
            builder.RegisterType<PromactErpContext>().As<DbContext>();

            // register webapi controller
            builder.RegisterApiControllers(typeof(LeaveRequestController).Assembly);
            builder.RegisterType<Program>().As<Program>().InstancePerLifetimeScope();

            // register repositories
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerDependency();
            builder.RegisterType<SlackRepository>().As<ISlackRepository>();
            builder.RegisterType<ScrumBotRepository>().As<IScrumBotRepository>();
            builder.RegisterType<Client>().As<IClient>();
            builder.RegisterType<ProjectUserCallRepository>().As<IProjectUserCallRepository>();
            builder.RegisterType<LeaveRequestRepository>().As<ILeaveRequestRepository>();
            builder.RegisterType<EmailService>().As<IEmailService>();
            builder.RegisterType<AttachmentRepository>().As<IAttachmentRepository>();
            builder.RegisterType<HttpClient>().InstancePerDependency();
            builder.RegisterType<HttpClientRepository>().As<IHttpClientRepository>();

            var container = builder.Build();

            // replace webapi dependancy resolver with autofac
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            return container;
        }
    }
}