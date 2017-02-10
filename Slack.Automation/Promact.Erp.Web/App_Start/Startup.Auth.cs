using Owin;
using Promact.Erp.DomainModel.Context;
using Promact.Erp.DomainModel.Models;
using Microsoft.Owin.Security.OpenIdConnect;
using Promact.Erp.Util;
using Promact.Erp.Util.EnvironmentVariableRepository;
using Autofac;
using Promact.Erp.Util.StringConstants;
using System.IdentityModel.Tokens;
using Promact.Core.Repository.ExternalLoginRepository;
using IdentityModel.Client;
using System.Collections.Generic;
using Microsoft.Owin.Security.Cookies;
using System.Threading.Tasks;
using System.Linq;
using Autofac.Extras.NLog;

namespace Promact.Erp.Web
{
    public partial class Startup
    {
        private IEnvironmentVariableRepository _environmentVariable;
        private IStringConstantRepository _stringConstantRepository;
        private IOAuthLoginRepository _oAuthLoginRepository;
        private ILogger _logger;
        private string _redirectUrl = null;
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864

        public void ConfigureAuth(IAppBuilder app, IComponentContext context)
        {
            _logger = context.Resolve<ILogger>();
            _environmentVariable = context.Resolve<IEnvironmentVariableRepository>();
            _oAuthLoginRepository = context.Resolve<IOAuthLoginRepository>();
            _stringConstantRepository = context.Resolve<IStringConstantRepository>();
            _redirectUrl = string.Format("{0}{1}", AppSettingUtil.PromactErpUrl, _stringConstantRepository.RedirectUrl);
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(PromactErpContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = _stringConstantRepository.AuthenticationType
            });
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {

                Authority = AppSettingUtil.OAuthUrl,
                ClientId = _environmentVariable.PromactOAuthClientId,
                ClientSecret = _environmentVariable.PromactOAuthClientSecret,
                RedirectUri = _redirectUrl,
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = _stringConstantRepository.NameClaimType,
                    RoleClaimType = _stringConstantRepository.RoleClaimType
                },

                ResponseType = _stringConstantRepository.ResponseType,
                Scope = _stringConstantRepository.Scope,
                SignInAsAuthenticationType = _stringConstantRepository.AuthenticationType,
                AuthenticationType = _stringConstantRepository.AuthenticationTypeOidc,
                PostLogoutRedirectUri = AppSettingUtil.PromactErpUrl,
                UseTokenLifetime = true,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenReceived = async tokenReceived =>
                    {
                        _logger.Info("Start With SecurityTokenReceived:");
                        var accessToken = tokenReceived.ProtocolMessage.AccessToken;
                        _logger.Info("AccessToken:" + accessToken);
                        var doc = await DiscoveryClient.GetAsync(AppSettingUtil.OAuthUrl);
                        _logger.Info("Doc:" + doc.UserInfoEndpoint);
                        var userInfoClient = new UserInfoClient(doc.UserInfoEndpoint);
                        var user = await userInfoClient.GetAsync(accessToken);
                        _logger.Info("user:" + user.Claims.Count());
                        _logger.Info("Doc TokenEndpoint:" + doc.TokenEndpoint);
                        _logger.Info("PromactOAuthClientId:" + _environmentVariable.PromactOAuthClientId);
                        _logger.Info("PromactOAuthClientSecret:" + _environmentVariable.PromactOAuthClientSecret);
                        var tokenClient = new TokenClient(doc.TokenEndpoint, _environmentVariable.PromactOAuthClientId, _environmentVariable.PromactOAuthClientSecret);
                        var response = await tokenClient.RequestAuthorizationCodeAsync(tokenReceived.ProtocolMessage.Code, _redirectUrl);
                        _logger.Info("Response:" + response.Error);
                        var refreshToken = response.RefreshToken;
                        _logger.Info("RefreshToken:" + refreshToken);
                        string userId = user.Claims.ToList().Single(x => x.Type == _stringConstantRepository.Sub).Value;
                        _logger.Info("UserId:" + userId);
                        string email = user.Claims.ToList().Single(x => x.Type == _stringConstantRepository.Email).Value;
                        _logger.Info("Email:" + email);
                        await _oAuthLoginRepository.AddNewUserFromExternalLoginAsync(email, refreshToken, userId);
                    },
                    AuthenticationFailed = authenticationFailed =>
                    {
                        authenticationFailed.Response.Redirect("/"); //redirect to home page.
                        authenticationFailed.HandleResponse();
                        return Task.FromResult(0);
                    },
                }
            });
        }
    }
}