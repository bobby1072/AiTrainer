using System.Net;
using AiTrainer.Web.Common.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AiTrainer.Web.Common.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid? GetCorrelationId(this HttpContext context)
        {
            string correlationId;

            correlationId = context.Request.Headers[ApiConstants.CorrelationIdHeader].ToString();

            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = context
                    .Response.Headers[ApiConstants.CorrelationIdHeader]
                    .ToString();
            }

            return !string.IsNullOrEmpty(correlationId) ? Guid.Parse(correlationId!) : null;
        }
        public static Guid GetDeviceToken(this HttpContext? context)
        {
            var deviceTokenHeader = context?.Request.Headers[ApiConstants.DeviceTokenHeader].ToString();

            if (string.IsNullOrEmpty(deviceTokenHeader))
            {
                throw new ApiException(
                        ExceptionConstants.NoDeviceTokenFound,
                        HttpStatusCode.BadRequest
                    );
            }
            return Guid.Parse(deviceTokenHeader);
        }
    }
}
