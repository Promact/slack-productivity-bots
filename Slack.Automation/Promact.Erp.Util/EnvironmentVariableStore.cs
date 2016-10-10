using System;

namespace Promact.Erp.Util
{
    public class EnvironmentVariableStore
    {

        public string Host
        {
            get
            {
                return Environment.GetEnvironmentVariable(StringConstant.Host, EnvironmentVariableTarget.Process);
            }
        }

        public string ScrumBotToken
        {
            get
            {
                return Environment.GetEnvironmentVariable(StringConstant.ScrumBotToken, EnvironmentVariableTarget.Process);
            }
        }

        public string PromactOAuthClientId
        {
            get
            {
                return Environment.GetEnvironmentVariable(StringConstant.PromactOAuthClientId, EnvironmentVariableTarget.Process);
            }
        }

        public string SlackOAuthClientId
        {
            get
            {
                return Environment.GetEnvironmentVariable(StringConstant.SlackOAuthClientId, EnvironmentVariableTarget.Process);
            }
        }

        public string Port
        {
            get
            {
                return Environment.GetEnvironmentVariable(StringConstant.Port, EnvironmentVariableTarget.Process);
            }
        }


        public string From
        {
            get
            {
                return Environment.GetEnvironmentVariable(StringConstant.From, EnvironmentVariableTarget.Process);
            }
        }


        public string Password
        {
            get
            {
                return Environment.GetEnvironmentVariable(StringConstant.Password, EnvironmentVariableTarget.Process);
            }
        }



        public string EnableSsl
        {
            get
            {
                return Environment.GetEnvironmentVariable(StringConstant.EnableSsl, EnvironmentVariableTarget.Process);
            }
        }


        public string SlackOAuthClientSecret
        {
            get
            {
                return Environment.GetEnvironmentVariable(StringConstant.SlackOAuthClientSecret, EnvironmentVariableTarget.Process);
            }
        }


        public string IncomingWebHookUrl
        {
            get
            {
                return Environment.GetEnvironmentVariable(StringConstant.IncomingWebHookUrl, EnvironmentVariableTarget.Process);
            }
        }


        public string PromactOAuthClientSecret
        {
            get
            {
                return Environment.GetEnvironmentVariable(StringConstant.PromactOAuthClientSecret, EnvironmentVariableTarget.Process);
            }
        }


        public string TaskmailAccessToken
        {
            get
            {
                return Environment.GetEnvironmentVariable(StringConstant.TaskmailAccessToken, EnvironmentVariableTarget.Process);
            }
        }
    }
}
