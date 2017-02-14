using Autofac.Extras.NLog;
using System.Web.Mvc;

namespace Promact.Erp.Core.ActionFilters
{
    public class ExceptionLoggerFilter : IExceptionFilter
    {
        private ILogger _iLogger;

        public ExceptionLoggerFilter(ILogger iLogger)
        {
            _iLogger = iLogger;
        }

        public void OnException(ExceptionContext context)
        {
            var response = GetResponse(context);

            _iLogger.Error("Error: " + response.StackTrace);
            _iLogger.Error("Error Message: " + response.Message);
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
