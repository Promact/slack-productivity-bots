
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

        public string ScrumBotToken
        {
            get
            {
                return "YourScrumBotToken";
            }
        }

        public string PromactOAuthClientId
        {
            get
            {
                return "YourPromactOAuthClientId";
            }
        }

        public string SlackOAuthClientId
        {
            get
            {
                return "YourSlackOAuthClientId";
            }
        }

        public string Port
        {
            get
            {
                return "1234";
            }
        }


        public string From
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


        public string SlackOAuthClientSecret
        {
            get
            {
                return "YourSlackOAuthClientSecret";
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


        public string TaskmailAccessToken
        {
            get
            {
                return "YourTaskmailAccessToken";
            }
        }
    }
}
