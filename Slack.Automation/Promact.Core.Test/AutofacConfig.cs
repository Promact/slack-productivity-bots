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

namespace Promact.Core.Test
{
    public class AutofacConfig
    {
        public static IComponentContext RegisterDependancies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<PromactErpContext>().As<DbContext>();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            builder.RegisterType<LeaveRequestRepository>().As<ILeaveRequestRepository>();
            builder.RegisterType<Client>().As<IClient>();
            builder.RegisterType<ProjectUserCallRepository>().As<IProjectUserCallRepository>();
            builder.RegisterType<SlackRepository>().As<ISlackRepository>();
            builder.RegisterType<Promact.Erp.Util.Email.EmailService>().As<IEmailService>();
            builder.RegisterType<AttachmentRepository>().As<IAttachmentRepository>();
            builder.RegisterType<HttpClient>();
            builder.RegisterType<HttpClientRepository>().As<IHttpClientRepository>();
            var container = builder.Build();
            return container;
        }
    }
}
