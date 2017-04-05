using Autofac;
using Promact.Core.Repository.AppCredentialRepository;
using Promact.Core.Repository.BotRepository;
using Promact.Erp.Util.StringConstants;
using System.Threading.Tasks;

namespace Promact.Erp.Web.App_Start
{
    public static class BotStartUp
    {
        public static async Task StartUpAsync(IComponentContext container)
        {
            IStringConstantRepository stringConstant = container.Resolve<IStringConstantRepository>();
            IAppCredentialRepository appCredential = container.Resolve<IAppCredentialRepository>();
            ISocketClientWrapper socketClientWrapper = container.Resolve<ISocketClientWrapper>();
            
            socketClientWrapper.InitializeAndConnectTaskBot((await appCredential.FetchAppCredentialByModuleAsync(stringConstant.TaskModule)).BotToken);
            socketClientWrapper.InitializeAndConnectScrumBot((await appCredential.FetchAppCredentialByModuleAsync(stringConstant.Scrum)).BotToken);
        }
    }
} 