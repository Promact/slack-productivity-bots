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
            //_userManager = context.Resolve<ApplicationUserManager>();
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
                Authority = /*AppSettingUtil.OAuthUrl*/"https://oauth.promactinfo.com/",
                ClientId = /* _environmentVariable.PromactOAuthClientId*/"HJXF74EQ497GRAL",
                ClientSecret = /*_environmentVariable.PromactOAuthClientSecret*/ "ChcBmvtaUwphIsbmRkbL9amOh9Qy6Q",
                RedirectUri = url,
                ResponseType = /*_stringConstantRepository.ResponseType*/"code id_token token",
                Scope = "email openid profile slack_user_id user_read" /*_stringConstantRepository.Scope*/,
                SignInAsAuthenticationType = _stringConstantRepository.AuthenticationType,
                AuthenticationType = _stringConstantRepository.AuthenticationTypeOidc,
                PostLogoutRedirectUri = AppSettingUtil.PromactErpUrl,
                UseTokenLifetime = true,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenReceived = tokenReceived =>
                    {
                        var accessToken = tokenReceived.ProtocolMessage.AccessToken;
                        //access the IdToken from Identity Server
                        var idToken = tokenReceived.ProtocolMessage.IdToken;
                        var refrenceToken = tokenReceived.ProtocolMessage.IdTokenHint;
                        var refrenceToken1 = tokenReceived.ProtocolMessage.AccessToken;
                        var UserName = tokenReceived.ProtocolMessage.Username;
                        var handler = new JwtSecurityTokenHandler();
                        var tokenS = handler.ReadToken(accessToken) as JwtSecurityToken;
                        string userId = null;
                        string email = null;
                        string slackUserId = null;
                        foreach (var claim in tokenS.Claims)
                        {
                            if (claim.Type == "sub")
                                userId = claim.Value;
                            if (claim.Type == "email")
                                email = claim.Value;
                            if (claim.Type == "slack_user_id")
                                slackUserId = claim.Value;
                        }
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