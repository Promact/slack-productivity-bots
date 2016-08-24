using System.Configuration;

namespace Promact.Erp.Util
{
    public class AppSettingsUtil
    {
        public static string ChatUpdateUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["SlackChatUpdateUrl"].ToString();
            }
        }

        public static string ProjectUserUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ProjectUserUrl"].ToString();
            }
        }


        public static string ProjectUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ProjectUrl"].ToString();
            }
        }


        public static string UserUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["UserUrl"].ToString();
            }
        }

        public static string IncomingWebHookUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["IncomingWebHookUrl"].ToString();
            }
        }

        public static string OAuthUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["OAuthUrl"].ToString();
            }
        }

        public static string ClientId
        {
            get
            {
                return ConfigurationManager.AppSettings["ClientId"].ToString();
            }
        }

        public static string ClientSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["ClientSecret"].ToString();
            }
        }

        public static string ClientReturnUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ClientReturnUrl"].ToString();
            }
        }

        public static string ProviderName
        {
            get
            {
                return ConfigurationManager.AppSettings["ProviderName"].ToString();
            }
        }

        public static string OAuthClientId
        {
            get
            {
                return ConfigurationManager.AppSettings["OAuthClientId"].ToString();
            }
        }

        public static string OAuthClientSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["OAuthClientSecret"].ToString();
            }
        }

        public static string LeaveManagementAuthorizationUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["LeaveManagementAuthorizationUrl"].ToString();
            }
        }
        public static string ChatPostUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ChatPostUrl"].ToString();
            }
        }
        public static string tsakmailId
        {
            get
            {
                return ConfigurationManager.AppSettings["tsakmailId"].ToString();
            }
        }
    }
}
