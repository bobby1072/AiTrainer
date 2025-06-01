using System.Net.Mime;
using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
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
            var time = await OperationTimerUtils.TimeAsync(() => TryToInvokeFuncAsync(context));
            var correlationId = context.GetCorrelationId();

            _logger.LogInformation(
                "Request with correlationId {CorrelationId} took {TimeTaken}ms to complete",
                correlationId,
                time.Milliseconds
            );
        }

        private async Task TryToInvokeFuncAsync(HttpContext context)
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
                var correlationId = context.GetCorrelationId();

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
                var correlationId = context.GetCorrelationId();

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
                var correlationId = context.GetCorrelationId();

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

        private static async Task RespondWithException(
            HttpContext context,
            ApiException apiException,
            Guid? correlationId = null
        )
        {
            context.Response.Clear();
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)apiException.StatusCode;
            if (correlationId is not null)
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
