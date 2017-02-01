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
using System.Web;
using System.Collections.Generic;

namespace Promact.Erp.Web
{
    public partial class Startup
    {
        private IEnvironmentVariableRepository _environmentVariable;
        private IStringConstantRepository _stringConstantRepository;
        private IOAuthLoginRepository _oAuthLoginRepository;
        private string redirectUrl = null;
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864

        public void ConfigureAuth(IAppBuilder app, IComponentContext context)
        {
            _environmentVariable = context.Resolve<IEnvironmentVariableRepository>();
            _oAuthLoginRepository = context.Resolve<IOAuthLoginRepository>();
            _stringConstantRepository = context.Resolve<IStringConstantRepository>();
            redirectUrl = string.Format("{0}{1}", AppSettingUtil.PromactErpUrl, _stringConstantRepository.RedirectUrl);
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(PromactErpContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            //app.UseExternalSignInCookie();

            //var url = string.Format("{0}{1}", AppSettingUtil.PromactErpUrl, _stringConstantRepository.RedirectUrl);
            //app.UsePromactAuthentication(new PromactAuthenticationOptions()
            //{
            //    Authority = AppSettingUtil.OAuthUrl,
            //    ClientId = "O1UGSJW6X4V16IY",
            //    ClientSecret = "RtZIZVt7VyW11NiSKazAxvTZlOpjRf",
            //    LogoutUrl = AppSettingUtil.PromactErpUrl,
            //    RedirectUrl = url,
            //    Notifications = new OpenIdConnectAuthenticationNotifications
            //    {
            //        SecurityTokenReceived = tokenReceived =>
            //        {
            //            var accessToken = tokenReceived.ProtocolMessage.AccessToken;
            //            var doc = DiscoveryClient.GetAsync(AppSettingUtil.OAuthUrl).Result;
            //            var userInfoClient = new UserInfoClient(doc.UserInfoEndpoint);

            //            var response = userInfoClient.GetAsync(accessToken).Result;
            //            var claims = response.Claims;
            //            var handler = new JwtSecurityTokenHandler();
            //            var tokenS = handler.ReadToken(accessToken) as JwtSecurityToken;
            //            string userId = null;
            //            string email = null;
            //            string slackUserId = null;
            //            foreach (var claim in tokenS.Claims)
            //            {
            //                if (claim.Type == _stringConstantRepository.Sub)
            //                    userId = claim.Value;
            //                if (claim.Type == _stringConstantRepository.Email)
            //                    email = claim.Value;
            //                if (claim.Type == _stringConstantRepository.SlackUserID)
            //                    slackUserId = claim.Value;
            //            }
            //            _oAuthLoginRepository.AddNewUserFromExternalLoginAsync(email, "", slackUserId, userId);
            //            return Task.FromResult(0);
            //        },
            //    }
            //});
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {

                Authority = AppSettingUtil.OAuthUrl,
                ClientId = /* _environmentVariable.PromactOAuthClientId*/"O1UGSJW6X4V16IY",
                ClientSecret = /*_environmentVariable.PromactOAuthClientSecret*/ "RtZIZVt7VyW11NiSKazAxvTZlOpjRf",
                RedirectUri = redirectUrl,
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                },

                ResponseType = /*_stringConstantRepository.ResponseType*/"code id_token token",
                Scope = "openid offline_access email profile slack_user_id user_read project_read" /*_stringConstantRepository.Scope*/,
                SignInAsAuthenticationType = _stringConstantRepository.AuthenticationType,
                AuthenticationType = _stringConstantRepository.AuthenticationTypeOidc,
                PostLogoutRedirectUri = AppSettingUtil.PromactErpUrl,
                UseTokenLifetime = true,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenReceived = tokenReceived =>
                    {
                        var accessToken = tokenReceived.ProtocolMessage.AccessToken;
                        var doc = DiscoveryClient.GetAsync(AppSettingUtil.OAuthUrl).Result;
                        var userInfoClient = new UserInfoClient(doc.UserInfoEndpoint);
                        var user = userInfoClient.GetAsync(accessToken).Result;
                        
                        var tokenClient = new TokenClient(doc.TokenEndpoint, "O1UGSJW6X4V16IY", "RtZIZVt7VyW11NiSKazAxvTZlOpjRf");
                        var response = tokenClient.RequestAuthorizationCodeAsync(tokenReceived.ProtocolMessage.Code, redirectUrl).Result;
                        var refreshToken = response.RefreshToken;
                        string userId = null;
                        string email = null;
                        string slackUserId = null;
                        foreach (var claim in user.Claims)
                        {
                            if (claim.Type == _stringConstantRepository.Sub)
                                userId = claim.Value;
                            if (claim.Type == _stringConstantRepository.Email)
                                email = claim.Value;
                            if (claim.Type == _stringConstantRepository.SlackUserID)
                                slackUserId = claim.Value;
                        }
                        _oAuthLoginRepository.AddNewUserFromExternalLoginAsync(email, refreshToken, slackUserId, userId);
                        return Task.FromResult(0);
                    },
                }
            });
        }
    }
}