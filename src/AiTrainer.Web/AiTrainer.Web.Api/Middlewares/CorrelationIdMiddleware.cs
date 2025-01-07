using AiTrainer.Web.Common;

namespace AiTrainer.Web.Api.Middlewares
{
    internal class CorrelationIdMiddleware : BaseMiddleware
    {
        public CorrelationIdMiddleware(RequestDelegate next) : base(next) { }

        public override async Task InvokeAsync(HttpContext context)
        {
            var correlationIdForRequest = Guid.NewGuid().ToString();

            context.Request.Headers.TryAdd(ApiConstants.CorrelationIdHeader, correlationIdForRequest);
            context.Response.Headers.TryAdd(ApiConstants.CorrelationIdHeader, correlationIdForRequest);

            await _next.Invoke(context);
        }
    }
}
