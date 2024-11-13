using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using System.Net.Mime;

namespace AiTrainer.Web.Api.Middlewares
{
    internal class ExceptionHandlingMiddleware : BaseMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger) : base(requestDelegate)
        {
            _logger = logger;
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
                return;
            }
            catch (ApiException e)
            {
                _logger.LogError(e, "ApiException was thrown during request for {Route} with message {Message}", context.Request.Path, e.Message);

                await RespondWithException(context, e);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Uncaught exception occured during request for {Route} with message {Message}", context.Request.Path, e.Message);

                await RespondWithException(context, new ApiException());
            }
        }
        private static async Task RespondWithException(HttpContext context, ApiException apiException)
        {
            context.Response.Clear();
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)apiException.StatusCode;
            var foundCorrelationId = context.GetCorrelationId();
            if (foundCorrelationId is not null)
            {
                context.Response.Headers.TryAdd(ApiConstants.CorrelationIdHeader, foundCorrelationId.ToString());
            }
            else
            {
                context.Response.Headers.TryAdd(ApiConstants.CorrelationIdHeader, Guid.NewGuid().ToString());
            }
            await context.Response.WriteAsJsonAsync(new Outcome { ExceptionMessage = apiException.Message, IsSuccess = false });
        }
    }
}
