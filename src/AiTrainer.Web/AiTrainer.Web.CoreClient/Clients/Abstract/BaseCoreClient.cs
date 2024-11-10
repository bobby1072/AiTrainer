using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.OperationTimer.Proto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace AiTrainer.Web.CoreClient.Clients.Abstract
{
    internal abstract class BaseCoreClient<TReturn> : ICoreClient<TReturn>
        where TReturn : class
    {
        protected readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
        protected readonly HttpClient _httpClient;
        protected string _operationName => GetType().Name;
        protected abstract string _endpoint { get; }
        protected ILogger<BaseCoreClient<TReturn>> _logger { get; init; }
        protected abstract CoreClientRequestType _requestType { get; }
        protected abstract HttpMethod _httpMethod { get; }

        protected BaseCoreClient(
            HttpClient httpClient,
            IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfig,
            ILogger<BaseCoreClient<TReturn>> logger
        )
        {
            _httpClient = httpClient;
            _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
            _logger = logger;
        }

        public virtual async Task<TReturn> InvokeAsync()
        {
            var requestMessage = BuildHttpMessage();

            var data = await InvokeCoreRequest(requestMessage);

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

        protected async Task<TReturn> InvokeCoreRequest(HttpRequestMessage request)
        {
            var response = await TimeAndExecuteRequest(() => _httpClient.SendAsync(request));

            response.EnsureSuccessStatusCodeAndThrowCoreClientException();

            var data = await response.Content.ReadFromJsonAsync<CoreResponse<TReturn>>();

            var actualData = data.EnsureSuccessfulCoreResponseAndGetData();

            return actualData;
        }

        protected HttpRequestMessage BuildHttpMessage()
        {
            HttpRequestMessage request;
            request = new HttpRequestMessage
            {
                Method = _httpMethod,
                RequestUri = _aiTrainerCoreConfiguration.BaseEndpoint.AppendPathToUrl(_endpoint),
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

    internal abstract class BaseCoreClient<TParam, TReturn>
        : BaseCoreClient<TReturn>,
            ICoreClient<TParam, TReturn>
        where TReturn : class
        where TParam : class
    {
        protected BaseCoreClient(
            HttpClient httpClient,
            IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfig,
            ILogger<BaseCoreClient<TReturn>> logger
        )
            : base(httpClient, aiTrainerCoreConfig, logger) { }

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
            if (_requestType == CoreClientRequestType.ApplicationJson)
            {
                var request = new HttpRequestMessage
                {
                    Method = _httpMethod,
                    Content = JsonContent.Create(param),
                    RequestUri = _aiTrainerCoreConfiguration.BaseEndpoint.AppendPathToUrl(
                        _endpoint
                    ),
                };
                AddApiKeyHeader(request);

                return request;
            }

            throw new NotImplementedException("Request type not implemented");
        }
    }
}
