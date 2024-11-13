using Microsoft.Extensions.Logging;
using System.Net;

namespace AiTrainer.Web.Common.Exceptions
{
    public class ApiException : AiTrainerException
    {
        public HttpStatusCode StatusCode { get; init; }
        public LogLevel LogLevel { get; init; } 
        public ApiException(
            string message = ExceptionConstants.InternalServerError,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            LogLevel logLevel = LogLevel.Error
        )
            : base(message)
        {
            StatusCode = statusCode;
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
