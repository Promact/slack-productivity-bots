using Microsoft.Owin;
using Owin;
using Promact.Erp.Web.App_Start;

[assembly: OwinStartupAttribute(typeof(Promact.Erp.Web.Startup))]
namespace Promact.Erp.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //var container = AutofacConfig.RegisterDependancies();
            ConfigureAuth(app);

        }
    }
}
