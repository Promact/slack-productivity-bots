using Promact.Erp.Util.StringConstants;
using System;

namespace Promact.Erp.Util.EnvironmentVariableRepository
{
    public class EnvironmentVariableRepository : IEnvironmentVariableRepository
    {

        #region Private Variable

        private readonly EnvironmentVariableTarget _EnvVariableTarget;
        private readonly IStringConstantRepository _stringConstant;

        #endregion


        #region Constructor

        public EnvironmentVariableRepository(IStringConstantRepository stringConstant)
        {
            _EnvVariableTarget = EnvironmentVariableTarget.Process;
            _stringConstant = stringConstant;
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
                return GetVariables(_stringConstant.Host);
            }
        }

        public string PromactOAuthClientId
        {
            get
            {
                return GetVariables(_stringConstant.PromactOAuthClientId);
            }
        }
              
        public int Port
        {
            get
            {
                return Convert.ToInt32(GetVariables(_stringConstant.Port));
            }
        }

        public string MailUserName
        {
            get
            {
                return GetVariables(_stringConstant.MailUserName);
            }
        }

        public string Password
        {
            get
            {
                return GetVariables(_stringConstant.Password);
            }
        }

        public bool EnableSsl
        {
            get
            {
                return Convert.ToBoolean(GetVariables(_stringConstant.EnableSsl));
            }
        }
              
        public string IncomingWebHookUrl
        {
            get
            {
                return GetVariables(_stringConstant.IncomingWebHookUrl);
            }
        }

        public string PromactOAuthClientSecret
        {
            get
            {
                return GetVariables(_stringConstant.PromactOAuthClientSecret);
            }
        }

    }
}
