using System.Configuration;

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
        public static string PromactErpUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["PromactErpUrl"].ToString();
            }
        }
    }
}
