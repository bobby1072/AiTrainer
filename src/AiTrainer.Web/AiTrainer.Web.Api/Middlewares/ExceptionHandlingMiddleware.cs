using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Services.Abstract;
using System.Net.Mime;

namespace AiTrainer.Web.Api.Middlewares
{
    internal class ExceptionHandlingMiddleware : BaseMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IApiRequestHttpContextService _requestContextService;
        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger,
            IApiRequestHttpContextService requestContextService) : base(requestDelegate)
        {
            _logger = logger;
            _requestContextService = requestContextService;
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
                _logger.Log(e.LogLevel, e, "ApiException was thrown during request for {Route} with message {Message} and status {Status} for correlationId {CorrelationId}", context.Request.Path, e.Message, e.StatusCode, _requestContextService.CorrelationId);

                await RespondWithException(context, e);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Uncaught exception occured during request for {Route} with message {Message} for correlationId {CorrelationId}", context.Request.Path, e.Message, _requestContextService.CorrelationId);

                await RespondWithException(context, new ApiException());
            }
        }
        private async Task RespondWithException(HttpContext context, ApiException apiException)
        {
            context.Response.Clear();
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)apiException.StatusCode;
            var foundCorrelationId = _requestContextService.CorrelationId;
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
