using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.Models
{

    /// <summary>
    /// Contains site's global variables whose values are obtained from environment variables.
    /// </summary>
    public static class GlobalClass
    {
        static string _scrumBotToken;
        static string _taskmailAccessToken;
        static string _promactOAuthClientSecret;
        static string _promactOAuthClientId;
        static string _host;
        static string _from;
        static string _port;
        static string _password;
        static string _enableSsl;
        static string _slackOAuthClientSecret;
        static string _slackOAuthClientId;
        static string _incomingWebHookUrl;

        public static string ScrumBotToken
        {
            get
            {
                return _scrumBotToken;
            }
            set
            {
                _scrumBotToken = Environment.GetEnvironmentVariable("ScrumBotToken", EnvironmentVariableTarget.Process);
            }
        }


        public static string PromactOAuthClientId
        {
            get
            {
                return _promactOAuthClientId;
            }
            set
            {
                _promactOAuthClientId = Environment.GetEnvironmentVariable("PromactOAuthClientId", EnvironmentVariableTarget.Process);
            }
        }


        public static string SlackOAuthClientId
        {

            get
            {
                return _slackOAuthClientId;
            }
            set
            {
                _slackOAuthClientId = Environment.GetEnvironmentVariable("SlackOAuthClientId", EnvironmentVariableTarget.Process);
            }
        }


        public static string SlackOAuthClientSecret
        {
            get
            {
                return _slackOAuthClientSecret;
            }
            set
            {
                _slackOAuthClientSecret = Environment.GetEnvironmentVariable("SlackOAuthClientSecret", EnvironmentVariableTarget.Process);
            }
        }


        public static string TaskmailAccessToken
        {
            get
            {
                return _taskmailAccessToken;
            }
            set
            {
                _taskmailAccessToken = Environment.GetEnvironmentVariable("DailyTaskMailAccessToken", EnvironmentVariableTarget.Process);
            }
        }


        public static string PromactOAuthClientSecret
        {
            get
            {
                return _promactOAuthClientSecret;
            }
            set
            {
                _promactOAuthClientSecret = Environment.GetEnvironmentVariable("PromactOAuthClientSecret", EnvironmentVariableTarget.Process);
            }
        }


        public static string Host
        {
            get
            {
                return _host;
            }
            set
            {
                _host = Environment.GetEnvironmentVariable("Host", EnvironmentVariableTarget.Process);
            }
        }


        public static string Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = Environment.GetEnvironmentVariable("Port", EnvironmentVariableTarget.Process);
            }
        }


        public static string From
        {
            get
            {
                return _from;
            }
            set
            {
                _from = Environment.GetEnvironmentVariable("From", EnvironmentVariableTarget.Process);
            }
        }


        public static string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = Environment.GetEnvironmentVariable("Password", EnvironmentVariableTarget.Process);
            }
        }


        public static string EnableSsl
        {
            get
            {
                return _enableSsl;
            }
            set
            {
                _enableSsl = Environment.GetEnvironmentVariable("EnableSsl", EnvironmentVariableTarget.Process);
            }
        }


        public static string IncomingWebHookUrl
        {
            get
            {
                return _incomingWebHookUrl;
            }
            set
            {
                _incomingWebHookUrl = Environment.GetEnvironmentVariable("IncomingWebHookUrl", EnvironmentVariableTarget.Process);
            }
        }

    }
}


