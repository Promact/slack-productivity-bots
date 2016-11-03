using Newtonsoft.Json;
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

            //JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            //{
            //    DefaultValueHandling = DefaultValueHandling.Ignore,
            //    MissingMemberHandling = MissingMemberHandling.Ignore,
            //    NullValueHandling = NullValueHandling.Ignore,
            //    //Error = (sender, args) =>
            //    //{
            //    //    //if (object.Equals(args.ErrorContext.Member, "gateway_allow_xmpp_ssl"))
            //    //    //{
            //    //    //    var sdf = args.ErrorContext.OriginalObject.GetType();
            //    //    args.ErrorContext.Handled = true;
            //    //    //  }
            //    //}
            //    // Formatting = Formatting.Indented,
            //    // TypeNameHandling = TypeNameHandling.Auto,

            //    // ContractResolver = new CamelCasePropertyNamesContractResolver()
            //};


            //JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            //{
            //    MissingMemberHandling = MissingMemberHandling.Ignore,
            //    Error = (sender, args1) =>
            //    {
            //        if (object.Equals(args1.ErrorContext.Member, "gateway_allow_xmpp_ssl"))
            //        {
            //            args1.ErrorContext.Handled = true;
            //        }
            //    }                
            //};

            Bot.ScrumMain(container);
            Bot.Main(container);



        }
    }
}
