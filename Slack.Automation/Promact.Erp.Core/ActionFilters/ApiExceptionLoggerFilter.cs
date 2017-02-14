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

        public Task ExecuteExceptionFilterAsync(
                HttpActionExecutedContext actionExecutedContext,
                CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                _logger.Error("Web Service Error Message:" + actionExecutedContext.Exception.Message);
                _logger.Error("Web Service Error StackTrace:" + actionExecutedContext.Exception.StackTrace);
            }, cancellationToken);
        }
    }
}
