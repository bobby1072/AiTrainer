using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;

internal class CoreClientCreateFaissStore : ICoreClient<CoreCreateFaissStoreInput, CoreFaissStoreResponse>
{
    private readonly ILogger<CoreClientCreateFaissStore> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;
    public CoreClientCreateFaissStore(
        ILogger<CoreClientCreateFaissStore> logger,
        HttpClient httpClient,
        IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfiguration,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfiguration.Value;
        _httpContextAccessor = httpContextAccessor;
        _httpClient = httpClient;
    }

    public async Task<CoreFaissStoreResponse?> TryInvokeAsync(CoreCreateFaissStoreInput param, CancellationToken cancellation = default)
    {
        try
        {
            var correlationId = _httpContextAccessor.HttpContext.GetCorrelationId();

            using var requestContent =
                CoreClientHttpExtensions.CreateApplicationJson(param, ApiConstants.DefaultCamelCaseSerializerOptions);

            using var httpResult = await _httpClient.SendWithRetry(
                requestMessage =>
                {
                    requestMessage.Method = HttpMethod.Post;
                    requestMessage.RequestUri = new Uri($"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/faissrouter/createstore");
                    requestMessage.Headers.AddApiKeyHeader(_aiTrainerCoreConfiguration.ApiKey);
                    requestMessage.Headers.AddCorrelationIdHeader(correlationId);
                    
                    requestMessage.Content = requestContent;
                },
                _aiTrainerCoreConfiguration,
                _logger,
                nameof(CoreClientCreateFaissStore),
                correlationId?.ToString(),
                cancellation
            );
            
            var finalResult = await httpResult.Content
                .TryDeserializeJson<CoreResponse<CoreFaissStoreResponse>>(ApiConstants.DefaultCamelCaseSerializerOptions, cancellation);
            
            return finalResult?.Data;
        }
        catch(Exception ex)
        {
            return null;
        }
    }
}