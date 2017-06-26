
namespace Promact.Erp.Util.EnvironmentVariableRepository
{
    public interface IEnvironmentVariableRepository
    {
        string ScrumBotToken { get; }

        string Host { get; }

        string PromactOAuthClientId { get; }

        string SlackOAuthClientId { get; }

        int Port { get; }

        string MailUserName { get; }

        string Password { get; }

        bool EnableSsl { get; }

        string SlackOAuthClientSecret { get; }

        string IncomingWebHookUrl { get; }

        string PromactOAuthClientSecret { get; }

        string TaskmailAccessToken { get; }

        string LeaveManagementBotAccessToken { get; }

        string FromMailAddressForTaskMailBot { get; }
    }
}
