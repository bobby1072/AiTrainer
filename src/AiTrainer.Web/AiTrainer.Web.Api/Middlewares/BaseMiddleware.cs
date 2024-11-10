namespace AiTrainer.Web.Api.Middlewares
{
    internal abstract class BaseMiddleware
    {
        protected readonly RequestDelegate _next;

        public BaseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public abstract Task InvokeAsync(HttpContext context);
    }
}
