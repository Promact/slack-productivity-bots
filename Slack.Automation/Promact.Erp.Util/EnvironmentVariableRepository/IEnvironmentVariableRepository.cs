
namespace Promact.Erp.Util.EnvironmentVariableRepository
{
    public interface IEnvironmentVariableRepository
    {      
        string Host { get; }

        string PromactOAuthClientId { get; }

        int Port { get; }

        string MailUserName { get; }

        string Password { get; }

        bool EnableSsl { get; }
             
        string IncomingWebHookUrl { get; }

        string PromactOAuthClientSecret { get; }
        
    }
}
