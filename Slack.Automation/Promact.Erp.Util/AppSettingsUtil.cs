using System.Configuration;

namespace Promact.Erp.Util
{
    public class AppSettingsUtil
    {
        /// <summary>
        /// Property to get SlackChatUpdateUrl from web config
        /// </summary>
        public static string ChatUpdateUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SlackChatUpdateUrl"].ToString();
            }
        }

        /// <summary>
        /// Property to get ProjectUserUrl from web config
        /// </summary>
        public static string ProjectUserUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ProjectUserUrl"].ToString();
            }
        }

        /// <summary>
        /// Property to get ProjectUrl from web config
        /// </summary>
        public static string ProjectUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ProjectUrl"].ToString();
            }
        }

        /// <summary>
        /// Property to get UserUrl from web config
        /// </summary>
        public static string UserUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["UserUrl"].ToString();
            }
        }

        /// <summary>
        /// Property to get IncomingWebHookUrl from web config
        /// </summary>
        public static string IncomingWebHookUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["IncomingWebHookUrl"].ToString();
            }
        }

        /// <summary>
        /// Property to get OAuthUrl from web config
        /// </summary>
        public static string OAuthUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["OAuthUrl"].ToString();
            }
        }

        /// <summary>
        /// Property to get ClientId from web config
        /// </summary>
        public static string ClientId
        {
            get
            {
                return ConfigurationManager.AppSettings["ClientId"].ToString();
            }
        }

        /// <summary>
        /// Property to get ClientSecret from web config
        /// </summary>
        public static string ClientSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["ClientSecret"].ToString();
            }
        }

        /// <summary>
        /// Property to get ClientReturnUrl from web config
        /// </summary>
        public static string ClientReturnUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ClientReturnUrl"].ToString();
            }
        }

        /// <summary>
        /// Property to get ProviderName from web config
        /// </summary>
        public static string ProviderName
        {
            get
            {
                return ConfigurationManager.AppSettings["ProviderName"].ToString();
            }
        }

        /// <summary>
        /// Property to get OAuthClientId from web config
        /// </summary>
        public static string OAuthClientId
        {
            get
            {
                return ConfigurationManager.AppSettings["OAuthClientId"].ToString();
            }
        }

        /// <summary>
        /// Property to get OAuthClientSecret from web config
        /// </summary>
        public static string OAuthClientSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["OAuthClientSecret"].ToString();
            }
        }

        /// <summary>
        /// Property to get LeaveManagementAuthorizationUrl from web config
        /// </summary>
        public static string LeaveManagementAuthorizationUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["LeaveManagementAuthorizationUrl"].ToString();
            }
        }

        /// <summary>
        /// Property to get ChatPostUrl from web config
        /// </summary>
        public static string ChatPostUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ChatPostUrl"].ToString();
            }
        }

        /// <summary>
        /// Property to get tsakmailId from web config
        /// </summary>
        public static string tsakmailId
        {
            get
            {
                return ConfigurationManager.AppSettings["tsakmailId"].ToString();
            }
        }

        /// <summary>
        /// Property to get tsakmail Access Token from web config
        /// </summary>
        public static string TaskmailAccessToken
        {
            get
            {
                return ConfigurationManager.AppSettings["TaskmailAccessToken"].ToString();
            }
        }
    }
}
