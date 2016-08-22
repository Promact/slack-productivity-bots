using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Promact.Erp.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
          // Web API configuration and services

         // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
              //routeTemplate: "api/{controller = Home}/{action = Index}/{id = RouteParameter.Optional}",
              //defaults: new { controller = "Home", action = "Index", id = RouteParameter.Optional }
            );

        }
    }
}
