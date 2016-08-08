using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Promact.Core.Repository.Client;
using Promact.Core.Repository.DataRepository;
using Promact.Core.Repository.LeaveRequestRepository;
using Promact.Core.Repository.ProjectUserCall;
using Promact.Core.Repository.SlackRepository;
using Promact.Erp.Core.Controllers;
using Promact.Erp.DomainModel.Context;
using Promact.Erp.Util.Email;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Promact.Erp.Web.App_Start
{
    public class AutofacConfig
    {
        public static IComponentContext RegisterDependancies()
        {

            var builder = new ContainerBuilder();
            // register dependency
            builder.RegisterType<PromactErpContext>().As<DbContext>();

            //register mvc controller
            //builder.RegisterControllers(typeof(HomeController).Assembly);

            // register webapi controller
            builder.RegisterApiControllers(typeof(LeaveRequestController).Assembly);

            // register repositories
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
            builder.RegisterType<LeaveRequestRepository>().As<ILeaveRequestRepository>();
            builder.RegisterType<SlackRepository>().As<ISlackRepository>();
            builder.RegisterType<Client>().As<IClient>();
            builder.RegisterType<ProjectUserCallRepository>().As<IProjectUserCallRepository>();
            builder.RegisterType<Promact.Erp.Util.Email.EmailService>().As<IEmailService>();

            var container = builder.Build();

            // replace mvc dependancy resolver with autofac
            //DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // replace webapi dependancy resolver with autofac
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            return container;
        }
    }
}