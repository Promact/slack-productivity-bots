using Microsoft.Owin.Security.Cookies;
using Owin;
using Promact.Erp.DomainModel.Context;
using Promact.Erp.DomainModel.Models;
using Microsoft.Owin.Security.OpenIdConnect;
using Promact.Erp.Util;
using Promact.Erp.Util.EnvironmentVariableRepository;
using Autofac;
using Promact.Erp.Util.StringConstants;

namespace Promact.Erp.Web
{
    public partial class Startup
    {
        private IEnvironmentVariableRepository _environmentVariable;
        private IStringConstantRepository _stringConstantRepository;
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864

        public void ConfigureAuth(IAppBuilder app, IComponentContext context)
        {
            _environmentVariable = context.Resolve<IEnvironmentVariableRepository>();
            _stringConstantRepository = context.Resolve<IStringConstantRepository>();
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(PromactErpContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);


            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = _stringConstantRepository.AuthenticationType
            });
            
            var url = string.Format("{0}{1}", AppSettingUtil.PromactErpUrl, "signin-oidc");
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = AppSettingUtil.OAuthUrl,
                ClientId =  _environmentVariable.PromactOAuthClientId,
                ClientSecret = _environmentVariable.PromactOAuthClientSecret,
                RedirectUri = url,
                ResponseType = _stringConstantRepository.ResponseType,
                Scope = _environmentVariable.Scopes, 
                SignInAsAuthenticationType = _stringConstantRepository.AuthenticationType,
                AuthenticationType = _stringConstantRepository.AuthenticationTypeOidc,
                PostLogoutRedirectUri = AppSettingUtil.PromactErpUrl,
                UseTokenLifetime = true,
                //Notifications
            });
        }
    }
}