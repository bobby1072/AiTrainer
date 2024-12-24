using System.Net.Mime;
using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Common.Models.ApiModels.Response;
using BT.Common.OperationTimer.Common;
using BT.Common.OperationTimer.Proto;

namespace AiTrainer.Web.Api.Middlewares
{
    internal class ExceptionHandlingMiddleware : BaseMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(
            RequestDelegate requestDelegate,
            ILogger<ExceptionHandlingMiddleware> logger
        )
            : base(requestDelegate)
        {
            _logger = logger;
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.GetCorrelationId();
            var time = await OperationTimerUtils.TimeAsync(() => TryInvokeAsync(context, correlationId?.ToString()));

            _logger.LogInformation("Request with correlationId {CorrelationId} took {TimeTaken}ms to complete", correlationId, time.Milliseconds);
        }

        public async Task TryInvokeAsync(HttpContext context, string? correlationId = null)
        {
            try
            {
                try
                {
                    await _next.Invoke(context);
                }
                catch (OperationTimerException e)
                {
                    if (e.InnerException is not null)
                    {
                        throw e.InnerException;
                    }

                    throw;
                }
            }
            catch (OperationTimerException e)
            {
                _logger.LogError(
                    e,
                    "Uncaught exception occured during request for {Route} with message {Message} for correlationId {CorrelationId}",
                    context.Request.Path,
                    e.Message,
                    correlationId
                );

                await RespondWithException(context, new ApiException(), correlationId);
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
                    correlationId
                );

                await RespondWithException(context, e, correlationId);
            }
            catch (Exception e)
            {
                _logger.LogError(
                    e,
                    "Uncaught exception occured during request for {Route} with message {Message} for correlationId {CorrelationId}",
                    context.Request.Path,
                    e.Message,
                    correlationId
                );

                await RespondWithException(context, new ApiException(), correlationId);
            }
        }

        private async Task RespondWithException(HttpContext context, ApiException apiException, string? correlationId = null)
        {
            context.Response.Clear();
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)apiException.StatusCode;
            if (correlationId is not null)
            {
                context.Response.Headers.TryAdd(
                    ApiConstants.CorrelationIdHeader,
                    correlationId
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
