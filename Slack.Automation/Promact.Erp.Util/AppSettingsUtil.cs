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
                return ConfigurationSettings.AppSettings["SlackChatUpdateUrl"];
            }
        }

        public static string ProjectUserUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["ProjectUserUrl"];
            }
        }

        public static string IncomingWebHookUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["IncomingWebHookUrl"];
            }
        }

        public static string OAuthUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["OAuthUrl"];
            }
        }

        public static string ClientId
        {
            get
            {
                return ConfigurationSettings.AppSettings["ClientId"];
            }
        }

        public static string ClientSecret
        {
            get
            {
                return ConfigurationSettings.AppSettings["ClientSecret"];
            }
        }

        public static string ClientReturnUrl
        {
            get
            {
                return ConfigurationSettings.AppSettings["ClientReturnUrl"];
            }
        }
    }
}
