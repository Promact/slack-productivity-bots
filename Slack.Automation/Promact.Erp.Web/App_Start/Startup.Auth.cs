using Owin;
using Promact.Erp.DomainModel.Context;
using Promact.Erp.DomainModel.Models;
using Microsoft.Owin.Security.OpenIdConnect;
using Promact.Erp.Util;
using Promact.Erp.Util.EnvironmentVariableRepository;
using Autofac;
using Promact.Erp.Util.StringConstants;
using System.Threading.Tasks;
using System.IdentityModel.Tokens;

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
            _userManager = context.Resolve<ApplicationUserManager>();
            _stringConstantRepository = context.Resolve<IStringConstantRepository>();
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(PromactErpContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = _stringConstantRepository.AuthenticationType
            });

            var url = string.Format("{0}{1}", AppSettingUtil.PromactErpUrl, "signin-oidc");
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = AppSettingUtil.OAuthUrl,
                ClientId = /* _environmentVariable.PromactOAuthClientId*/"Z6UFWD97GNN3G6F",
                ClientSecret = /*_environmentVariable.PromactOAuthClientSecret*/ "UVgC7d221yhydWPpF7UxHhsiYV2lS7",
                RedirectUri = url,
                ResponseType = _stringConstantRepository.ResponseType,
                Scope = "email openid profile slack_user_id user_read" /*_stringConstantRepository.Scope*/,
                SignInAsAuthenticationType = _stringConstantRepository.AuthenticationType,
                AuthenticationType = _stringConstantRepository.AuthenticationTypeOidc,
                PostLogoutRedirectUri = AppSettingUtil.PromactErpUrl,
                UseTokenLifetime = true,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenReceived = tokenReceived =>
                    {
                        //access the IdToken from Identity Server
                        var idToken = tokenReceived.ProtocolMessage.IdToken;
                        var refrenceToken = tokenReceived.ProtocolMessage.IdTokenHint;
                        var refrenceToken1 = tokenReceived.ProtocolMessage.AccessToken;
                        var UserName = tokenReceived.ProtocolMessage.Username;
                        var handler = new JwtSecurityTokenHandler();
                        var tokenS = handler.ReadToken(idToken) as JwtSecurityToken;
                        var userId = tokenS.Payload.Sub;
                        return Task.FromResult(0);
                    },
                    AuthenticationFailed = authenticationFailed =>
                    {
                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}