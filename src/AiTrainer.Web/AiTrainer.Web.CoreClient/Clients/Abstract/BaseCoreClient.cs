using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.HttpClient.Extensions;
using BT.Common.OperationTimer.Proto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using BT.Common.Polly.Models.Concrete;

namespace AiTrainer.Web.CoreClient.Clients.Abstract
{
    internal abstract class BaseCoreClient
    {
        private readonly ILogger<BaseCoreClient> _logger;
        protected readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
        protected readonly HttpClient _httpClient;
        protected string _operationName => GetType().Name;

        protected BaseCoreClient(
            HttpClient httpClient,
            IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfig,
            ILogger<BaseCoreClient> logger)
        {
            _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
            _logger = logger;
            _httpClient = httpClient;
        }

        protected async Task<TReturn> InvokeCoreRequest<TReturn>(HttpRequestMessage request) where TReturn : BaseCoreClientResponseBody
        {
            
            var totalAttempts = _aiTrainerCoreConfiguration.TotalAttempts > 1 ? _aiTrainerCoreConfiguration.TotalAttempts : 2;
            var timeoutInSeconds = _aiTrainerCoreConfiguration.TimeoutInSeconds > 3 ? _aiTrainerCoreConfiguration.TimeoutInSeconds : 90;
            var delay = _aiTrainerCoreConfiguration.DelayBetweenAttemptsInSeconds >= 0 ? _aiTrainerCoreConfiguration.DelayBetweenAttemptsInSeconds : 1;
            
            var response = await TimeAndExecuteRequest(() => _httpClient.SendAsync(request, new PollyRetrySettings
            {
                TotalAttempts = totalAttempts,
                DelayBetweenAttemptsInSeconds = delay,
                TimeoutInSeconds = timeoutInSeconds,
            }));

            response.EnsureSuccessStatusCodeAndThrowCoreClientException();

            var data = await response.Content.ReadFromJsonAsync<CoreResponse<TReturn>>();

            var actualData = data.EnsureSuccessfulCoreResponseAndGetData();

            return actualData;
        }

        protected void AddApiKeyHeader(HttpRequestMessage requestMessage)
        {
            if (!requestMessage.Headers.Contains(CoreClientConstants.ApiKeyHeader))
            {
                requestMessage.Headers.Add(
                    CoreClientConstants.ApiKeyHeader,
                    _aiTrainerCoreConfiguration.ApiKey
                );
            }
        }

        protected void LogCoreError(Exception exception)
        {
            _logger.LogError(
                exception,
                "Exception in core occurred while making {MethodName} request. With exception message {ExceptionMessage}",
                _operationName,
                exception.Message
            );
        }
        protected async Task<T> TimeAndExecuteRequest<T>(Func<Task<T>> request)
        {
            var (time, result) = await OperationTimerUtils.TimeWithResultsAsync(request);

            _logger.LogDebug(
                "Core request {MethodName} took {ElapsedMilliseconds}ms to complete",
                _operationName,
                time.Milliseconds
            );

            return result;
        }
    }
    internal abstract class BaseCoreClient<TReturn> : BaseCoreClient ,ICoreClient<TReturn> 
        where TReturn : BaseCoreClientResponseBody
    {

        private ILogger<BaseCoreClient<TReturn>> _logger { get; init; }
        protected BaseCoreClient(
            HttpClient httpClient,
            IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfig,
            ILogger<BaseCoreClient<TReturn>> logger
        ): base(httpClient, aiTrainerCoreConfig, logger)
        {
            _logger = logger;
        }
        protected abstract HttpRequestMessage BuildMessage();

        public virtual async Task<TReturn> InvokeAsync()
        {
            var message = BuildMessage();
            AddApiKeyHeader(message);
            
            var data = await InvokeCoreRequest<TReturn>(message);

            return data;
        }

        public virtual async Task<TReturn?> TryInvokeAsync()
        {
            try
            {
                return await InvokeAsync();
            }
            catch (Exception coreClientException)
            {
                LogCoreError(coreClientException);
                return null;
            }
        }

        
    }

    internal abstract class BaseCoreClient<TParam, TReturn>
        : BaseCoreClient,
            ICoreClient<TParam, TReturn>
        where TParam : BaseCoreClientRequestBody
        where TReturn : BaseCoreClientResponseBody
    {
        protected BaseCoreClient(
            HttpClient httpClient,
            IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfig,
            ILogger<BaseCoreClient<TParam ,TReturn>> logger
        )
            : base(httpClient, aiTrainerCoreConfig, logger) { }
        protected abstract HttpRequestMessage BuildMessage(TParam param);
        public virtual async Task<TReturn> InvokeAsync(TParam param)
        {
            var message = BuildMessage(param);
            AddApiKeyHeader(message);
            
            var data = await InvokeCoreRequest<TReturn>(message);

            return data;
        }

        public virtual async Task<TReturn?> TryInvokeAsync(TParam param)
        {
            try
            {
                return await InvokeAsync(param);
            }
            catch (Exception coreClientException)
            {
                LogCoreError(coreClientException);
                return null;
            }
        }
    }
}
