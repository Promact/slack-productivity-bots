using System;

namespace Promact.Erp.Util.EnvironmentVariableRepository
{
    public class EnvironmentVariableRepository : IEnvironmentVariableRepository
    {

        #region Private Variable

        private readonly EnvironmentVariableTarget _EnvVariableTarget;

        #endregion


        #region Constructor

        public EnvironmentVariableRepository()
        {
            _EnvVariableTarget = EnvironmentVariableTarget.Process;
        }

        #endregion


        #region Private Method

        private string GetVariables(string VariableName)
        {
            return Environment.GetEnvironmentVariable(VariableName, _EnvVariableTarget);
        }

        #endregion


        public string Host
        {
            get
            {
                return GetVariables(StringConstant.Host);
            }
        }

        public string ScrumBotToken
        {
            get
            {
                return GetVariables(StringConstant.ScrumBotToken);
            }
        }

        public string PromactOAuthClientId
        {
            get
            {
                return GetVariables(StringConstant.PromactOAuthClientId);
            }
        }

        public string SlackOAuthClientId
        {
            get
            {
                return GetVariables(StringConstant.SlackOAuthClientId);
            }
        }

        public int Port
        {
            get
            {
                return Convert.ToInt32(GetVariables(StringConstant.Port));
            }
        }

        public string From
        {
            get
            {
                return GetVariables(StringConstant.From);
            }
        }

        public string Password
        {
            get
            {
                return GetVariables(StringConstant.Password);
            }
        }

        public bool EnableSsl
        {
            get
            {
                return Convert.ToBoolean(GetVariables(StringConstant.EnableSsl));
            }
        }

        public string SlackOAuthClientSecret
        {
            get
            {
                return GetVariables(StringConstant.SlackOAuthClientSecret);
            }
        }

        public string IncomingWebHookUrl
        {
            get
            {
                return GetVariables(StringConstant.IncomingWebHookUrl);
            }
        }

        public string PromactOAuthClientSecret
        {
            get
            {
                return GetVariables(StringConstant.PromactOAuthClientSecret);
            }
        }

        public string TaskmailAccessToken
        {
            get
            {
                return GetVariables(StringConstant.TaskmailAccessToken);
            }
        }
    }
}
