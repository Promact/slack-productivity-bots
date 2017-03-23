using Autofac;
using Promact.Core.Repository.AppCredentialRepository;
using Promact.Core.Repository.BotRepository;
using Promact.Erp.Util.StringConstants;
using System.Threading.Tasks;

namespace Promact.Erp.Web
{
    public static class BotStartUp
    {
        public static async Task StartUpAsync(IComponentContext container)
        {
            IStringConstantRepository stringConstant = container.Resolve<IStringConstantRepository>();
            IAppCredentialRepository appCredential = container.Resolve<IAppCredentialRepository>();
            ITaskMailBotRepository taskMailBotRepository = container.Resolve<ITaskMailBotRepository>();
            IScrumRepository scrumBotRepository = container.Resolve<IScrumRepository>();
            taskMailBotRepository.StartAndConnectTaskMailBot((await appCredential.FetchAppCredentialByModule(stringConstant.TaskModule)).BotToken);
            scrumBotRepository.StartAndConnectScrumBot((await appCredential.FetchAppCredentialByModule(stringConstant.Scrum)).BotToken);
        }
    }
}