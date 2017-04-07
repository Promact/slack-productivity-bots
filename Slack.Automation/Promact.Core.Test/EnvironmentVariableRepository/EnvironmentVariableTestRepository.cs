
using Promact.Erp.Util.EnvironmentVariableRepository;

namespace Promact.Core.Test.EnvironmentVariableRepository
{
    public class EnvironmentVariableTestRepository : IEnvironmentVariableRepository
    {

        #region Constructor

        public EnvironmentVariableTestRepository()
        {
        }

        #endregion
        

        public string Host
        {
            get
            {
                return "YourHostName";
            }
        }

        public string PromactOAuthClientId
        {
            get
            {
                return "YourPromactOAuthClientId";
            }
        }

        public int Port
        {
            get
            {
                return 1234;
            }
        }


        public string MailUserName
        {
            get
            {
                return "FromWhomSoever";
            }
        }


        public string Password
        {
            get
            {
                return "abc";
            }
        }



        public bool EnableSsl
        {
            get
            {
                return true;
            }
        }


        public string IncomingWebHookUrl
        {
            get
            {
                return "YourIncomingWebHookUrl";
            }
        }


        public string PromactOAuthClientSecret
        {
            get
            {
                return "YourPromactOAuthClientSecret";
            }
        }

    }
}
