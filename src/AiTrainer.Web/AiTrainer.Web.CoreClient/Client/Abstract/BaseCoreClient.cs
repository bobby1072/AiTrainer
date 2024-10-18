using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.OperationTimer.Proto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Client.Concrete
{
    internal abstract class BaseCoreClient<TParam, TReturn>
        where TReturn : class
    {
        protected const string _applicationJson = "application/json";
        protected readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
        protected readonly ILogger _logger;
        protected readonly HttpClient _httpClient;

        public BaseCoreClient(
            HttpClient httpClient,
            ILogger<BaseCoreClient<TParam, TReturn>> logger,
            IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfig
        )
        {
            _httpClient = httpClient;
            _logger = logger;
            _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
        }

        public abstract Task<TReturn> InvokeAsync(TParam param);

        public async Task<TReturn?> TryInvokeAsync(TParam param)
        {
            try
            {
                return await InvokeAsync(param);
            }
            catch (Exception coreClientException)
            {
                LogCoreError(coreClientException, nameof(CoreClientChunkDocument));
                return null;
            }
        }

        protected async Task<TReturn> ExecuteRequest(
            CoreClientRequestType requestType,
            HttpMethod httpMethod,
            string endpoint,
            string operationName,
            TParam? param
        )
        {
            var requestMessage = BuildHttpMessage(requestType, httpMethod, endpoint, param);

            var data = await InvokeCoreRequest(requestMessage, operationName);

            return data;
        }

        private void AddApiKeyHeader(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Add(
                CoreClientConstants.ApiKeyHeader,
                _aiTrainerCoreConfiguration.ApiKey
            );
        }

        private async Task<TReturn> InvokeCoreRequest(HttpRequestMessage request, string methodName)
        {
            var response = await TimeAndExecuteRequest(
                () => _httpClient.SendAsync(request),
                methodName
            );

            response.EnsureSuccessStatusCodeAndThrowCoreClientException();

            var data = await response.Content.ReadFromJsonAsync<CoreResponse<TReturn>>();

            var actualData = data.EnsureSuccessfulCoreResponseAndGetData();

            return actualData;
        }

        private void LogCoreError(Exception exception, string methodName)
        {
            _logger.LogError(
                exception,
                "Exception in core occurred while making {MethodName} request. With exception message {ExceptionMessage}",
                methodName,
                exception.Message
            );
        }

        private HttpRequestMessage BuildHttpMessage(
            CoreClientRequestType requestType,
            HttpMethod httpMethod,
            string endpoint,
            TParam? param
        )
        {
            HttpRequestMessage request;
            if (requestType == CoreClientRequestType.Json && param is not null)
            {
                request = new HttpRequestMessage
                {
                    Method = httpMethod,
                    Content = new StringContent(
                        JsonSerializer.Serialize(param),
                        Encoding.UTF8,
                        _applicationJson
                    ),
                    RequestUri = new Uri($"{_aiTrainerCoreConfiguration.BaseEndpoint}/{endpoint}"),
                };
            }
            else
            {
                request = new HttpRequestMessage
                {
                    Method = httpMethod,
                    RequestUri = new Uri($"{_aiTrainerCoreConfiguration.BaseEndpoint}/{endpoint}"),
                };
            }

            AddApiKeyHeader(request);

            return request;
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
