using Autofac.Extras.NLog;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Promact.Erp.Core.ActionFilters
{
    public class ApiExceptionLoggerFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        public ApiExceptionLoggerFilter(ILogger logger)
        {
            _logger = logger;
        }

        public bool AllowMultiple { get { return true; } }

        public async Task ExecuteExceptionFilterAsync(
                HttpActionExecutedContext actionExecutedContext,
                CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(() =>
            {
               _logger.Error(actionExecutedContext.Exception);
            }, cancellationToken);
        }
    }
}
