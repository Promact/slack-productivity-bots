using Microsoft.Owin;
using Owin;
using Promact.Erp.Web.App_Start;
using Autofac;
using Autofac.Extras.NLog;
using Promact.Erp.Core.ActionFilters;
using System.Web.Http;
using System.Web.Mvc;


[assembly: OwinStartupAttribute(typeof(Promact.Erp.Web.Startup))]
namespace Promact.Erp.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = AutofacConfig.RegisterDependancies();
            DatabaseConfig.Initialize(container);
            GlobalFilters.Filters.Add(new ExceptionLoggerFilter(container.Resolve<ILogger>()));
            GlobalConfiguration.Configuration.Filters.Add(new ApiExceptionLoggerFilter(container.Resolve<ILogger>()));
            ConfigureAuth(app, container);
            BotStartUp.StartUpAsync(container).Wait();
        }
    }
}
