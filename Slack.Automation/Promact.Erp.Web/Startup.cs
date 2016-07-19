using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Promact.Erp.Web.Startup))]
namespace Promact.Erp.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
