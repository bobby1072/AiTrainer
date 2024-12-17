using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Domain.Services.Abstract;
using BT.Common.OperationTimer.Proto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AiTrainer.Web.Domain.Services.Concrete
{
    internal class DomainServiceActionExecutor : IDomainServiceActionExecutor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DomainServiceActionExecutor> _logger;
        private readonly IApiRequestHttpContextService _apiRequestHttpContextService;
        public DomainServiceActionExecutor(
            IServiceProvider serviceProvider,
            ILogger<DomainServiceActionExecutor> logger,
            IApiRequestHttpContextService apiRequestHttpContextService
        )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _apiRequestHttpContextService = apiRequestHttpContextService;
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
            var correlationId = _apiRequestHttpContextService.CorrelationId;
            _logger.LogInformation(
                "-------Entering service action executor for {ServiceAction} for correlationId {CorrelationId}-------",
                actionName,
                correlationId
            );

            var service =
                _serviceProvider.GetRequiredService<TService>();

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

            var correlationId = _apiRequestHttpContextService.CorrelationId;
            _logger.LogInformation(
                "-------Entering service action {ServiceAction} for correlationId {CorrelationId}-------",
                actionName,
                correlationId
            );

            var service =
                _serviceProvider.GetRequiredService<TService>();

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
