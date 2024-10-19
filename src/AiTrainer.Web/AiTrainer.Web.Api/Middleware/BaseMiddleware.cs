namespace AiTrainer.Web.Api.Middleware
{
    internal abstract class BaseMiddleware
    {
        protected readonly RequestDelegate _next;

        public BaseMiddleware(RequestDelegate next)
        {
            _next = next;
        }
    }
}
