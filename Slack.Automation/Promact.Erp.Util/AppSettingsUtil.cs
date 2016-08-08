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
    }
}
