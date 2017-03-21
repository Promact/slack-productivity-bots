using Autofac;
using Autofac.Extras.NLog;
using Promact.Core.Repository.BotRepository;
using Promact.Erp.Core.ActionFilters;
using Promact.Erp.Core.Controllers;
using Promact.Erp.Util.StringConstants;
using Promact.Erp.Web.App_Start;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


namespace Promact.Erp.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = AutofacConfig.RegisterDependancies();
            DatabaseConfig.Initialize(container);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalFilters.Filters.Add(new ExceptionLoggerFilter(container.Resolve<ILogger>()));
            GlobalConfiguration.Configuration.Filters.Add(new ApiExceptionLoggerFilter(container.Resolve<ILogger>()));
            IStringConstantRepository stringConstant = container.Resolve<IStringConstantRepository>();
            IBotRepository botRepository = container.Resolve<IBotRepository>();
            botRepository.TurnOnBot(stringConstant.TaskModule).Wait();
            botRepository.TurnOnBot(stringConstant.Scrum).Wait();
        }

    }
}
