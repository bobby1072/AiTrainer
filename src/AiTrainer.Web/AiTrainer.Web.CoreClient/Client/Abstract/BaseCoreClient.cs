using System.Net.Http.Json;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.OperationTimer.Proto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Client.Concrete
{
    internal abstract class BaseCoreClient
    {
        protected const string _applicationJson = "application/json";
        protected readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
        protected readonly ILogger _logger;
        protected readonly HttpClient _httpClient;

        public BaseCoreClient(
            HttpClient httpClient,
            ILogger<BaseCoreClient> logger,
            IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfig
        )
        {
            _httpClient = httpClient;
            _logger = logger;
            _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
        }

        protected void AddApiKeyHeader(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Add(
                CoreClientConstants.ApiKeyHeader,
                _aiTrainerCoreConfiguration.ApiKey
            );
        }

        protected async Task<T> InvokeCoreRequest<T>(HttpRequestMessage request, string methodName)
        {
            var response = await TimeAndExecuteRequest(
                () => _httpClient.SendAsync(request),
                methodName
            );

            response.EnsureSuccessStatusCodeAndThrowCoreClientException();

            var data = await response.Content.ReadFromJsonAsync<CoreResponse<T>>();

            var actualData = data.EnsureSuccessfulCoreResponseAndGetData();

            return actualData;
        }

        protected void LogCoreError(Exception exception, string methodName)
        {
            _logger.LogError(
                exception,
                "Exception in core occurred while making {MethodName} request. With exception message {ExceptionMessage}",
                methodName,
                exception.Message
            );
        }

        private async Task<T> TimeAndExecuteRequest<T>(Func<Task<T>> request, string methodName)
        {
            var (time, result) = await OperationTimerUtils.TimeWithResultsAsync(request);

            _logger.LogInformation(
                "Core request {MethodName} took {ElapsedMilliseconds}ms",
                methodName,
                time.Milliseconds
            );

            return result;
        }
    }
}
