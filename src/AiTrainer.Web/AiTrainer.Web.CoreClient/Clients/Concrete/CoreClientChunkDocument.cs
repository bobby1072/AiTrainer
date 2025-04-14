using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AiTrainer.Web.Common;
using BT.Common.Polly.Extensions;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;
internal class CoreClientChunkDocument : ICoreClient<CoreDocumentToChunkInput, CoreChunkedDocumentResponse>
{
    private readonly ILogger<CoreClientChunkDocument> _logger;
    private readonly HttpClient _httpClient;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CoreClientChunkDocument(
        ILogger<CoreClientChunkDocument> logger,
        HttpClient httpClient,
        IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfig,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _httpClient = httpClient;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<CoreChunkedDocumentResponse?> TryInvokeAsync(CoreDocumentToChunkInput param, CancellationToken cancellationToken = default)
    {
        using var requestContent = CoreClientHttpExtensions.CreateApplicationJson(param, ApiConstants.DefaultCamelCaseSerializerOptions);
        using var requestMessage = new HttpRequestMessage();
        requestMessage.Method = HttpMethod.Post;
        requestMessage.RequestUri =
            new Uri($"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/chunkingrouter/chunkdocument");
        requestMessage.Headers.AddApiKeyHeader(_aiTrainerCoreConfiguration.ApiKey);
        requestMessage.Headers.AddCorrelationIdHeader(_httpContextAccessor.HttpContext.GetCorrelationId());
        
        requestMessage.Content = requestContent;
        
        
        var retryPipeline = _aiTrainerCoreConfiguration.ToPipeline();
        var result = await retryPipeline.ExecuteAsync(async ct =>
            {
                var response = await _httpClient.SendAsync(requestMessage,
                    ct);
                response.EnsureSuccessStatusCode();

                return await response.Content
                    .TryDeserializeJson<CoreResponse<CoreChunkedDocumentResponse>>(
                        ApiConstants.DefaultCamelCaseSerializerOptions, cancellationToken);
            }, cancellationToken)
            .AsTask()
            .CoreClientExceptionHandling(_logger, nameof(CoreClientChunkDocument));

        return result?.Data;
    }
}

