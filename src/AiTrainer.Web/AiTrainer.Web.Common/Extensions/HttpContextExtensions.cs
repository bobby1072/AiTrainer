using AiTrainer.Web.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace AiTrainer.Web.Common.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid? GetCorrelationId(this HttpContext context)
        {
            context.Request.Headers.TryGetValue(
                ApiConstants.CorrelationIdHeader,
                out var correlationId
            );

            if (string.IsNullOrEmpty(correlationId))
            {
                context.Response.Headers.TryGetValue(
                    ApiConstants.CorrelationIdHeader,
                    out var correlationIdFromResponse
                );
                correlationId = correlationIdFromResponse;
            }

            return !string.IsNullOrEmpty(correlationId.ToString())
                ? Guid.Parse(correlationId!)
                : null;
        }

        public static string? GetAccessTokenOrNull(this HttpContext? context)
        {
            try
            {
                return context.GetAccessToken();
            }
            catch
            {
                return null;
            }
        }

        public static string GetAccessTokenFromQuery(this HttpContext? context, string keyName)
        {
            var token = context?.Request.Query[keyName].FirstOrDefault();
            
            if (string.IsNullOrEmpty(token))
            {
                throw new ApiException(
                    ExceptionConstants.Unauthorized,
                    HttpStatusCode.Unauthorized
                );
            }

            return token.RemoveBearerPrefix();
        } 
        public static string GetAccessToken(this HttpContext? context)
        {
            var token = context?.Request.Headers[HeaderNames.Authorization].ToString();
            if (string.IsNullOrEmpty(token))
            {
                throw new ApiException(
                    ExceptionConstants.Unauthorized,
                    HttpStatusCode.Unauthorized
                );
            }

            return token.RemoveBearerPrefix();
        }
        private static string RemoveBearerPrefix(this string token) => token.Replace("bearer ", string.Empty, StringComparison.OrdinalIgnoreCase);
    }
}
