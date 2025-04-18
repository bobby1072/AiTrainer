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
        try
        {
            var correlationId = _httpContextAccessor.HttpContext.GetCorrelationId();
            
            using var requestContent =
                CoreClientHttpExtensions.CreateApplicationJson(param, ApiConstants.DefaultCamelCaseSerializerOptions);

            using var httpResult = await _httpClient.SendWithRetry(
                requestMessage =>
                {
                    requestMessage.Method = HttpMethod.Post;
                    requestMessage.RequestUri =
                        new Uri($"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/chunkingrouter/chunkdocument");
                    requestMessage.Headers.AddApiKeyHeader(_aiTrainerCoreConfiguration.ApiKey);
                    requestMessage.Headers.AddCorrelationIdHeader(correlationId);

                    requestMessage.Content = requestContent;
                }, 
                _aiTrainerCoreConfiguration,
                _logger,
                nameof(CoreClientChunkDocument),
                correlationId?.ToString(),
                cancellationToken);

            var finalResult = await httpResult.Content
                .TryDeserializeJson<CoreResponse<CoreChunkedDocumentResponse>>(
                    ApiConstants.DefaultCamelCaseSerializerOptions, cancellationToken);

            return finalResult?.Data;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Exception occured in {OpName} with message {Message}",
                nameof(CoreClientChunkDocument),
                ex.Message);
            
            return null;
        }
    }
}

