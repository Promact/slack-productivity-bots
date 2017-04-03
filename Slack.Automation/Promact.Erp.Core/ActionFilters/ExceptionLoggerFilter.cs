using Autofac.Extras.NLog;
using System.Web.Mvc;

namespace Promact.Erp.Core.ActionFilters
{
    public class ExceptionLoggerFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        public ExceptionLoggerFilter(ILogger logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var response = GetResponse(context);
            _logger.Error(response.Message);
            _logger.Trace(response.StackTrace);
        }

        private ErrorResponse GetResponse(ExceptionContext context)
        {
            return new ErrorResponse()
            {
                Message = context.Exception.Message,
                StackTrace = context.Exception.StackTrace
            };
        }

        public class ErrorResponse
        {
            public ErrorResponse()
            {
            }

            public string Message { get; set; }
            public string StackTrace { get; set; }
        }
    }
}
