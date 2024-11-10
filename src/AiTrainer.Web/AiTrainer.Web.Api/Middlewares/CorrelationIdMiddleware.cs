namespace AiTrainer.Web.Api.Middlewares
{
    internal class CorrelationIdMiddleware : BaseMiddleware
    {
        public CorrelationIdMiddleware(RequestDelegate next) : base(next) { }

        public override async Task InvokeAsync(HttpContext context)
        {
            var correlationIdforRequest = Guid.NewGuid().ToString();

            context.Request.Headers.TryAdd(ApiConstants.CorrelationIdHeader, correlationIdforRequest);
            context.Response.Headers.TryAdd(ApiConstants.CorrelationIdHeader, correlationIdforRequest);

            await _next.Invoke(context);
        }
    }
}
