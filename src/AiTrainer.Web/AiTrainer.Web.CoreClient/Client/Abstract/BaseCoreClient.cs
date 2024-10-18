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

        public virtual Task<TReturn> InvokeAsync() => ExecuteRequest();

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

        protected async Task<TReturn> ExecuteRequest()
        {
            var requestMessage = BuildHttpMessage();

            var data = await InvokeCoreRequest(requestMessage);

            return data;
        }

        protected async Task<T> TimeAndExecuteRequest<T>(Func<Task<T>> request)
        {
            var (time, result) = await OperationTimerUtils.TimeWithResultsAsync(request);

            _logger.LogInformation(
                "Core request {MethodName} took {ElapsedMilliseconds}ms",
                _operationName,
                time.Milliseconds
            );

            return result;
        }

        protected async Task<TReturn> InvokeCoreRequest(HttpRequestMessage request)
        {
            var response = await TimeAndExecuteRequest(() => _httpClient.SendAsync(request));

            response.EnsureSuccessStatusCodeAndThrowCoreClientException();

            var data = await response.Content.ReadFromJsonAsync<CoreResponse<TReturn>>();

            var actualData = data.EnsureSuccessfulCoreResponseAndGetData();

            return actualData;
        }

        protected virtual HttpRequestMessage BuildHttpMessage()
        {
            HttpRequestMessage request;
            request = new HttpRequestMessage
            {
                Method = _httpMethod,
                RequestUri = new Uri($"{_aiTrainerCoreConfiguration.BaseEndpoint}/{_endpoint}"),
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

        protected void LogCoreError(Exception exception)
        {
            _logger.LogError(
                exception,
                "Exception in core occurred while making {MethodName} request. With exception message {ExceptionMessage}",
                _operationName,
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
            var data = await ExecuteRequest(param);

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

        protected async Task<TReturn> ExecuteRequest(TParam param)
        {
            var requestMessage = BuildHttpMessage(param);

            var data = await InvokeCoreRequest(requestMessage);

            return data;
        }

        protected HttpRequestMessage BuildHttpMessage(TParam param)
        {
            var request = new HttpRequestMessage
            {
                Method = _httpMethod,
                Content = new StringContent(
                    JsonSerializer.Serialize(param),
                    Encoding.UTF8,
                    _applicationJson
                ),
                RequestUri = new Uri($"{_aiTrainerCoreConfiguration.BaseEndpoint}/{_endpoint}"),
            };
            AddApiKeyHeader(request);

            return request;
        }
    }
}
