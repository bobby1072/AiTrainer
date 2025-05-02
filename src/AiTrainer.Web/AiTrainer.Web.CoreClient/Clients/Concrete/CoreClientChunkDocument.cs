using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.Http.Extensions;
using BT.Common.Polly.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;

internal class CoreClientChunkDocument
    : ICoreClient<CoreDocumentToChunkInput, CoreChunkedDocumentResponse>
{
    private readonly ILogger<CoreClientChunkDocument> _logger;
    private readonly HttpClient _httpClient;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CoreClientChunkDocument(
        ILogger<CoreClientChunkDocument> logger,
        HttpClient httpClient,
        IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfig,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _httpClient = httpClient;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CoreChunkedDocumentResponse?> TryInvokeAsync(
        CoreDocumentToChunkInput param,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var correlationId = _httpContextAccessor.HttpContext.GetCorrelationId();

            var pipeline = _aiTrainerCoreConfiguration.ToPipeline();
            
            // WHEN SEMANTIC CHUNKING WORKS WILL ASSIGN FROM CONFIG
            // param.CoreDocumentToChunkInputChunkingType = (CoreDocumentToChunkInputChunkingTypeEnum)_aiTrainerCoreConfiguration.DocumentChunkingType;
            param.CoreDocumentToChunkInputChunkingType = CoreDocumentToChunkInputChunkingTypeEnum.Recursive;
            
            var response = await pipeline.ExecuteAsync(async ct => await _aiTrainerCoreConfiguration.BaseEndpoint
                .AppendPathSegment("api")
                .AppendPathSegment("chunkingrouter")
                .AppendPathSegment("chunkdocument")
                .WithCoreApiKeyHeader(_aiTrainerCoreConfiguration.ApiKey)
                .WithCorrelationIdHeader(correlationId?.ToString())
                .WithApplicationJson(param, ApiConstants.DefaultCamelCaseSerializerOptions)
                .PostJsonAsync<CoreResponse<CoreChunkedDocumentResponse>>(_httpClient,
                    ApiConstants.DefaultCamelCaseSerializerOptions, ct), cancellationToken);
            

            return response?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Exception occured in {OpName} with message {Message}",
                nameof(CoreClientChunkDocument),
                ex.Message
            );

            return null;
        }
    }
}
