using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Services.Abstract;
using BT.Common.OperationTimer.Proto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.Concrete
{
    internal class DomainServiceActionExecutor : IDomainServiceActionExecutor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DomainServiceActionExecutor> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DomainServiceActionExecutor(
            IServiceProvider serviceProvider,
            ILogger<DomainServiceActionExecutor> logger,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TReturn> ExecuteAsync<TService, TReturn>(
            Func<TService, Task<TReturn>> serviceAction
        )
            where TService : IDomainService
        {
            var correlationId = _httpContextAccessor.HttpContext.GetCorrelationId();
            _logger.LogInformation(
                "-------Entering service action {ServiceAction} for correlationId {CorrelationId}-------",
                serviceAction.Method.Name,
                correlationId
            );

            var service =
                _serviceProvider.GetService<TService>()
                ?? throw new InvalidOperationException(ExceptionConstants.NoService);

            var (timeTaken, result) = await OperationTimerUtils.TimeWithResultsAsync(
                () => serviceAction.Invoke(service)
            );

            _logger.LogInformation(
                "Service action {ServiceAction} for correlationId {CorrelationId} completed in {TimeTaken}ms",
                serviceAction.Method.Name,
                correlationId,
                timeTaken.Milliseconds
            );

            _logger.LogInformation(
                "-------Exiting service action {ServiceAction} successfully for correlationId {CorrelationId}-------",
                serviceAction.Method.Name,
                correlationId
            );

            return result;
        }

        public TReturn Execute<TService, TReturn>(Func<TService, TReturn> serviceAction)
            where TService : IDomainService
        {
            var correlationId = _httpContextAccessor.HttpContext.GetCorrelationId();
            _logger.LogInformation(
                "-------Entering service action {ServiceAction} for correlationId {CorrelationId}-------",
                serviceAction.Method.Name,
                correlationId
            );

            var service =
                _serviceProvider.GetService<TService>()
                ?? throw new InvalidOperationException(ExceptionConstants.NoService);

            var (timeTaken, result) = OperationTimerUtils.TimeWithResults(
                () => serviceAction.Invoke(service)
            );

            _logger.LogInformation(
                "Service action {ServiceAction} for correlationId {CorrelationId} completed in {TimeTaken}ms",
                serviceAction.Method.Name,
                correlationId,
                timeTaken.Milliseconds
            );

            _logger.LogInformation(
                "-------Exiting service action {ServiceAction} for correlationId {CorrelationId}-------",
                serviceAction.Method.Name,
                correlationId
            );

            return result;
        }
    }
}