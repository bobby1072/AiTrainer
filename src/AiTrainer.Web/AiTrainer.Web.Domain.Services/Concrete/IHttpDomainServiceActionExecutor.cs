using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Services.Abstract;
using BT.Common.OperationTimer.Proto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AiTrainer.Web.Domain.Services.Concrete
{
    internal class HttpDomainServiceActionExecutor : IHttpDomainServiceActionExecutor
    {
        private readonly ILogger<IHttpDomainServiceActionExecutor> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;
        public HttpDomainServiceActionExecutor(
            ILogger<IHttpDomainServiceActionExecutor> logger,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task ExecuteAsync<TService>(
            Expression<Func<TService, Task>> serviceAction,
            string? serviceActionName = null
        )
            where TService : IDomainService
        {
            var compiledAction = serviceAction.Compile();
            await ExecuteAsync((Func<TService, Task<bool>>)(async serv =>
            {
                await compiledAction.Invoke(serv);
                return true;
            }), serviceActionName);
        }
        public Task<TReturn> ExecuteAsync<TService, TReturn>(
            Expression<Func<TService, Task<TReturn>>> serviceAction,
            string? serviceActionName = null
        )
            where TService : IDomainService => ExecuteAsync(serviceAction.Compile(), serviceActionName);
        private async Task<TReturn> ExecuteAsync<TService, TReturn>(
            Func<TService, Task<TReturn>> serviceAction,
            string? serviceActionName = null
        )
            where TService : IDomainService
        {
            var actionName = serviceActionName ?? serviceAction.Method.Name;
            var correlationId = _httpContextAccessor.HttpContext?.GetCorrelationId();
            _logger.LogInformation(
                "-------Entering service action executor for {ServiceAction} for correlationId {CorrelationId}-------",
                actionName,
                correlationId
            );

            var service = _serviceProvider.GetRequiredService<TService>();

            var (timeTaken, result) = await OperationTimerUtils.TimeWithResultsAsync(
                () => serviceAction.Invoke(service)
            );

            if (service is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else if (service is IDisposable disposable)
            {
                disposable.Dispose();
            }

            
            _logger.LogInformation(
                "Service action {ServiceAction} for correlationId {CorrelationId} completed in {TimeTaken}ms",
                actionName,
                correlationId,
                timeTaken.Milliseconds
            );

            _logger.LogInformation(
                "-------Exiting service action executor for {ServiceAction} successfully for correlationId {CorrelationId}-------",
                actionName,
                correlationId
            );

            return result;
        }
    }
}
