using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Services.Abstract;
using BT.Common.OperationTimer.Proto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AiTrainer.Web.Domain.Services.Concrete
{
    internal class DomainServiceActionExecutor : IDomainServiceActionExecutor
    {
        private readonly ILogger<DomainServiceActionExecutor> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DomainServiceActionExecutor(
            ILogger<DomainServiceActionExecutor> logger,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public Task<TReturn> ExecuteAsync<TService, TReturn>(
            Expression<Func<TService, Task<TReturn>>> serviceAction,
            string? serviceActionName = null
        )
            where TService : IDomainService => ExecuteAsync(serviceAction.Compile(), serviceActionName);
        public TReturn Execute<TService, TReturn>(
            Expression<Func<TService, TReturn>> serviceAction,
            string? serviceActionName = null
        )
            where TService : IDomainService => Execute(serviceAction.Compile(), serviceActionName);

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

            var service = _httpContextAccessor.HttpContext!.RequestServices.GetService<TService>() ?? throw new InvalidOperationException("No service");

            var (timeTaken, result) = await OperationTimerUtils.TimeWithResultsAsync(
                () => serviceAction.Invoke(service)
            );

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
        private TReturn Execute<TService, TReturn>(
            Func<TService, TReturn> serviceAction,
            string? serviceActionName = null
        )
            where TService : IDomainService
        {
            var actionName = serviceActionName ?? serviceAction.Method.Name;

            var correlationId = _httpContextAccessor.HttpContext?.GetCorrelationId();
            _logger.LogInformation(
                "-------Entering service action {ServiceAction} for correlationId {CorrelationId}-------",
                actionName,
                correlationId
            );

            var service = _httpContextAccessor.HttpContext!.RequestServices.GetService<TService>() ?? throw new InvalidOperationException("No service");

            var (timeTaken, result) = OperationTimerUtils.TimeWithResults(
                () => serviceAction.Invoke(service)
            );

            _logger.LogInformation(
                "Service action {ServiceAction} for correlationId {CorrelationId} completed in {TimeTaken}ms",
                actionName,
                correlationId,
                timeTaken.Milliseconds
            );

            _logger.LogInformation(
                "-------Exiting service action {ServiceAction} for correlationId {CorrelationId}-------",
                actionName,
                correlationId
            );

            return result;
        }
    }
}
