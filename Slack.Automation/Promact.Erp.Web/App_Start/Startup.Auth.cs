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
using Promact.Core.Repository.ExternalLoginRepository;
using IdentityModel.Client;
using System.Security.Claims;
using Microsoft.Owin.Security;

namespace Promact.Erp.Web
{
    public partial class Startup
    {
        private IEnvironmentVariableRepository _environmentVariable;
        private IStringConstantRepository _stringConstantRepository;
        private IOAuthLoginRepository _oAuthLoginRepository;
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864

        public void ConfigureAuth(IAppBuilder app, IComponentContext context)
        {
            _environmentVariable = context.Resolve<IEnvironmentVariableRepository>();
            _oAuthLoginRepository = context.Resolve<IOAuthLoginRepository>();
            _stringConstantRepository = context.Resolve<IStringConstantRepository>();
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(PromactErpContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = _stringConstantRepository.AuthenticationType
            });

            var url = string.Format("{0}{1}", AppSettingUtil.PromactErpUrl, _stringConstantRepository.RedirectUrl);
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {

                Authority = AppSettingUtil.OAuthUrl,
                ClientId = /* _environmentVariable.PromactOAuthClientId*/"Z6UFWD97GNN3G6F",
                ClientSecret = /*_environmentVariable.PromactOAuthClientSecret*/ "UVgC7d221yhydWPpF7UxHhsiYV2lS7",
                RedirectUri = url,

                ResponseType = /*_stringConstantRepository.ResponseType*/"code id_token token",
                Scope = "email openid profile slack_user_id user_read offline_access" /*_stringConstantRepository.Scope*/,
                SignInAsAuthenticationType = _stringConstantRepository.AuthenticationType,
                AuthenticationType = _stringConstantRepository.AuthenticationTypeOidc,
                PostLogoutRedirectUri = AppSettingUtil.PromactErpUrl,
                UseTokenLifetime = true,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenReceived = tokenReceived =>
                    {
                        var accessToken = tokenReceived.ProtocolMessage.AccessToken;
                        var handler = new JwtSecurityTokenHandler();
                        var tokenS = handler.ReadToken(accessToken) as JwtSecurityToken;
                        string userId = null;
                        string email = null;
                        string slackUserId = null;
                        foreach (var claim in tokenS.Claims)
                        {
                            if (claim.Type == _stringConstantRepository.Sub)
                                userId = claim.Value;
                            if (claim.Type == _stringConstantRepository.Email)
                                email = claim.Value;
                            if (claim.Type == _stringConstantRepository.SlackUserID)
                                slackUserId = claim.Value;
                        }
                        _oAuthLoginRepository.AddNewUserFromExternalLoginAsync(email, "", slackUserId, userId);
                        return Task.FromResult(0);
                    },
                    //AuthorizationCodeReceived = async n =>
                    //{
                    //    var discoveryClient = new DiscoveryClient(AppSettingUtil.OAuthUrl);
                    //    var doc = await discoveryClient.GetAsync();
                    //    // use the code to get the access and refresh token
                    //    var tokenClient = new TokenClient(
                    //        doc.TokenEndpoint,
                    //        "Z6UFWD97GNN3G6F",
                    //        "UVgC7d221yhydWPpF7UxHhsiYV2lS7");

                    //    var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(
                    //    n.Code, n.RedirectUri);

                    //    // create new identity
                    //    var id = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType);
                    //    id.AddClaim(new Claim("access_token", tokenResponse.AccessToken));
                    //    id.AddClaim(new Claim("refresh_token", tokenResponse.RefreshToken));
                    //    id.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                    //    id.AddClaim(new Claim("sid", n.AuthenticationTicket.Identity.FindFirst("sid").Value));

                    //    n.AuthenticationTicket = new AuthenticationTicket(
                    //        new ClaimsIdentity(id.Claims, n.AuthenticationTicket.Identity.AuthenticationType, "name", "role"),
                    //        n.AuthenticationTicket.Properties);
                    //},
                }
            });
        }
    }
}