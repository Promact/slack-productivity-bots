using System.Web.Mvc;
using System.Web.Routing;

namespace Promact.Erp.Web
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //    );

            routes.MapRoute(
              "Default",
             "{controller}/{action}/{id}",
             new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
             "AcceptAll",
             "{*url}",
             new { controller = "Home", action = "Index" }

            );
        }
    }
}
