using System.Net;

namespace AiTrainer.Web.Common.Exceptions
{
    public class ApiException : AiTrainerException
    {
        public HttpStatusCode StatusCode { get; init; }

        public ApiException(
            string message = ExceptionConstants.InternalServerError,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError
        )
            : base(message)
        {
            StatusCode = statusCode;
        }

        public ApiException(
            Exception innerException,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            string message = ExceptionConstants.InternalServerError
        )
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
