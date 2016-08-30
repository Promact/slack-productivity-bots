using System.Configuration;

namespace Promact.Erp.Util
{
    public class AppSettingsUtil
    {
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
