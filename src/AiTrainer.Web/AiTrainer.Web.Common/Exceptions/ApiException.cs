using System.Net;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Common.Exceptions
{
    public sealed class ApiException : AiTrainerException
    {
        public HttpStatusCode StatusCode { get; }
        public LogLevel LogLevel { get; }

        public ApiException(
            string message = ExceptionConstants.InternalServerError,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            LogLevel logLevel = LogLevel.Error
        )
            : base(message)
        {
            StatusCode = statusCode;
            LogLevel = logLevel;
        }

        public ApiException(
            LogLevel logLevel,
            string message = ExceptionConstants.InternalServerError,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError
        )
            : base(message)
        {
            StatusCode = statusCode;
            LogLevel = LogLevel;
        }
    }
}
