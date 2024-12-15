using System.Net.Mime;
using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Common.Models.ApiModels.Response;
using AiTrainer.Web.Domain.Services.Abstract;
using BT.Common.OperationTimer.Proto;

namespace AiTrainer.Web.Api.Middlewares
{
    internal class ExceptionHandlingMiddleware : BaseMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(
            RequestDelegate requestDelegate,
            ILogger<ExceptionHandlingMiddleware> logger,
            IApiRequestHttpContextService requestContextService
        )
            : base(requestDelegate)
        {
            _logger = logger;
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            var time = await OperationTimerUtils.TimeAsync(() => TryInvokeAsync(context));

            _logger.LogInformation("Request with correlationId {CorrelationId} took {TimeTaken}ms to complete", context.GetCorrelationId(), time.Milliseconds);
        }

        private async Task TryInvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
                return;
            }
            catch (ApiException e)
            {
                _logger.Log(
                    e.LogLevel,
                    e,
                    "ApiException was thrown during request for {Route} with message {Message} and status {Status} for correlationId {CorrelationId}",
                    context.Request.Path,
                    e.Message,
                    e.StatusCode,
                    context.GetCorrelationId()
                );

                await RespondWithException(context, e);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Uncaught exception occured during request for {Route} with message {Message} for correlationId {CorrelationId}",
                    context.Request.Path,
                    e.Message,
                    context.GetCorrelationId()
                );

                await RespondWithException(context, new ApiException());
            }
        }

        private async Task RespondWithException(HttpContext context, ApiException apiException)
        {
            var correlationId = context.Response.Headers[ApiConstants.CorrelationIdHeader].ToString();
            context.Response.Clear();
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)apiException.StatusCode;
            if (!string.IsNullOrEmpty(correlationId))
            {
                context.Response.Headers.TryAdd(
                    ApiConstants.CorrelationIdHeader,
                    correlationId.ToString()
                );
            }
            else
            {
                context.Response.Headers.TryAdd(
                    ApiConstants.CorrelationIdHeader,
                    Guid.NewGuid().ToString()
                );
            }
            await context.Response.WriteAsJsonAsync(
                new Outcome { ExceptionMessage = apiException.Message }
            );
        }
    }
}
