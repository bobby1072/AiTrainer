using System.Net;
using AiTrainer.Web.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

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

        public static string GetAccessToken(this HttpContext context) =>
            context.Request.Headers[HeaderNames.Authorization].ToString()
            ?? throw new ApiException(
                ExceptionConstants.NotAuthorized,
                HttpStatusCode.Unauthorized
            );

        public static Guid GetDeviceToken(this HttpContext context) =>
            Guid.Parse(
                context.Request.Headers[ApiConstants.DeviceTokenHeader].ToString()
                    ?? throw new ApiException(
                        ExceptionConstants.NoDeviceTokenFound,
                        HttpStatusCode.BadRequest
                    )
            );
    }
}
