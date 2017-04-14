using Microsoft.Owin;
using Owin;
using Promact.Erp.Web.App_Start;
using Autofac;
using Autofac.Extras.NLog;
using Promact.Erp.Core.ActionFilters;
using Promact.Erp.Core.Controllers;
using System.Web.Http;
using System.Web.Mvc;
using Promact.Erp.Util.StringLiteral;

[assembly: OwinStartupAttribute(typeof(Promact.Erp.Web.Startup))]
namespace Promact.Erp.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = AutofacConfig.RegisterDependancies();
            DatabaseConfig.Initialize(container);
            IStringLiteral stringLiteral = container.Resolve<IStringLiteral>();
            stringLiteral.OnInit();
            stringLiteral.CreateFileWatcher();
            GlobalFilters.Filters.Add(new ExceptionLoggerFilter(container.Resolve<ILogger>()));
            GlobalConfiguration.Configuration.Filters.Add(new ApiExceptionLoggerFilter(container.Resolve<ILogger>()));
            Bot bot = container.Resolve<Bot>();
            bot.Scrum();
            bot.TaskMailBot();
            ConfigureAuth(app, container);

        }
    }
}
