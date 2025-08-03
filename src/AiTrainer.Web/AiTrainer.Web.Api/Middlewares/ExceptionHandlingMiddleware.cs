using System.Net.Mime;
using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using BT.Common.OperationTimer.Common;
using BT.Common.OperationTimer.Proto;
using CommonApiConstants = BT.Common.Api.Helpers.ApiConstants;


namespace AiTrainer.Web.Api.Middlewares
{
    internal sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(
            RequestDelegate requestDelegate
        )
        {
            _next = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<ExceptionHandlingMiddleware> logger)
        {
            var time = await OperationTimerUtils.TimeAsync(() => TryToInvokeFuncAsync(context, logger));
            var correlationId = context.GetCorrelationId();

            logger.LogInformation(
                "Request with correlationId {CorrelationId} took {TimeTaken}ms to complete",
                correlationId,
                time.Milliseconds
            );
        }

        private async Task TryToInvokeFuncAsync(HttpContext context, ILogger<ExceptionHandlingMiddleware> logger)
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

                logger.LogError(
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

                logger.Log(
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

                logger.LogError(
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
                    CommonApiConstants.CorrelationIdHeader,
                    correlationId.ToString()
                );
            }
            else
            {
                context.Response.Headers.TryAdd(
                    CommonApiConstants.CorrelationIdHeader,
                    Guid.NewGuid().ToString()
                );
            }
            await context.Response.WriteAsJsonAsync(
                new Outcome { ExceptionMessage = apiException.Message }
            );
        }
    }
}
