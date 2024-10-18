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

namespace AiTrainer.Web.CoreClient.Client.Abstract
{
    internal abstract class BaseCoreClient<TReturn>
        where TReturn : class
    {
        protected const string _applicationJson = "application/json";
        protected readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
        protected readonly HttpClient _httpClient;
        protected string _operationName => GetType().Name;
        protected abstract string _endpoint { get; }
        protected abstract ILogger _logger { get; init; }
        protected abstract CoreClientRequestType _requestType { get; }
        protected abstract HttpMethod _httpMethod { get; }

        protected BaseCoreClient(
            HttpClient httpClient,
            IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfig
        )
        {
            _httpClient = httpClient;
            _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
        }

        public virtual async Task<TReturn> InvokeAsync()
        {
            var data = await ExecuteRequest(_requestType, _httpMethod, _endpoint, _operationName);

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
                LogCoreError(coreClientException, _operationName);
                return null;
            }
        }

        protected async Task<TReturn> ExecuteRequest(
            CoreClientRequestType requestType,
            HttpMethod httpMethod,
            string endpoint,
            string operationName
        )
        {
            var requestMessage = BuildHttpMessage(requestType, httpMethod, endpoint);

            var data = await InvokeCoreRequest(requestMessage, operationName);

            return data;
        }

        protected async Task<T> TimeAndExecuteRequest<T>(Func<Task<T>> request, string methodName)
        {
            var (time, result) = await OperationTimerUtils.TimeWithResultsAsync(request);

            _logger.LogInformation(
                "Core request {MethodName} took {ElapsedMilliseconds}ms",
                methodName,
                time.Milliseconds
            );

            return result;
        }

        protected async Task<TReturn> InvokeCoreRequest(
            HttpRequestMessage request,
            string methodName
        )
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

        protected virtual HttpRequestMessage BuildHttpMessage(
            CoreClientRequestType requestType,
            HttpMethod httpMethod,
            string endpoint
        )
        {
            HttpRequestMessage request;
            request = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = new Uri($"{_aiTrainerCoreConfiguration.BaseEndpoint}/{endpoint}"),
            };

            return request;
        }

        protected void AddApiKeyHeader(HttpRequestMessage requestMessage)
        {
            requestMessage?.Headers.Add(
                CoreClientConstants.ApiKeyHeader,
                _aiTrainerCoreConfiguration.ApiKey
            );
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
    }

    internal abstract class BaseCoreClient<TParam, TReturn> : BaseCoreClient<TReturn>
        where TReturn : class
    {
        protected BaseCoreClient(
            HttpClient httpClient,
            IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfig
        )
            : base(httpClient, aiTrainerCoreConfig) { }

        public virtual async Task<TReturn> InvokeAsync(TParam param)
        {
            var data = await ExecuteRequest(_requestType, _httpMethod, _endpoint, _operationName);

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
                LogCoreError(coreClientException, _operationName);
                return null;
            }
        }

        protected async Task<TReturn> ExecuteRequest(
            CoreClientRequestType requestType,
            HttpMethod httpMethod,
            string endpoint,
            string operationName,
            TParam param
        )
        {
            var requestMessage = BuildHttpMessage(requestType, httpMethod, endpoint, param);

            var data = await InvokeCoreRequest(requestMessage, operationName);

            return data;
        }

        protected HttpRequestMessage BuildHttpMessage(
            CoreClientRequestType requestType,
            HttpMethod httpMethod,
            string endpoint,
            TParam param
        )
        {
            var request = new HttpRequestMessage
            {
                Method = httpMethod,
                Content = new StringContent(
                    JsonSerializer.Serialize(param),
                    Encoding.UTF8,
                    _applicationJson
                ),
                RequestUri = new Uri($"{_aiTrainerCoreConfiguration.BaseEndpoint}/{endpoint}"),
            };
            AddApiKeyHeader(request);

            return request;
        }
    }
}
