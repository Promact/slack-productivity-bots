using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.Util
{
    public class AppSettingUtil
    {
        public static string OAuthUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["OAuthUrl"].ToString();
            }
        }
        public static string PromactErp
        {
            get
            {
                return ConfigurationManager.AppSettings["PromactErp"].ToString();
            }
        }
    }
}
