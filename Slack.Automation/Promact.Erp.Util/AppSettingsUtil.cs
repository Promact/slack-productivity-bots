using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
