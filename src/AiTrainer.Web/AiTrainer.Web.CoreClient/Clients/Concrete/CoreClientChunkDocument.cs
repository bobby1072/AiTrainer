﻿using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.HttpClient.Extensions;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;
internal class CoreClientChunkDocument : ICoreClient<DocumentToChunkInput, ChunkedDocumentResponse>
{
    private readonly ILogger<CoreClientChunkDocument> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CoreClientChunkDocument(
        ILogger<CoreClientChunkDocument> logger,
        IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfig,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<ChunkedDocumentResponse?> TryInvokeAsync(DocumentToChunkInput param)
    {
        var response = await _aiTrainerCoreConfiguration.BaseEndpoint
            .AppendPathSegment("api")
            .AppendPathSegment("chunkingrouter")
            .AppendPathSegment("chunkdocument")
            .WithAiTrainerCoreKeyHeader(_aiTrainerCoreConfiguration.ApiKey)
            .WithCorrelationIdHeader(_httpContextAccessor.HttpContext.GetCorrelationId())
            .PostJsonAsync(param)
            .ReceiveJsonAsync<CoreResponse<ChunkedDocumentResponse>>(_aiTrainerCoreConfiguration)
            .CoreClientExceptionHandling(_logger, nameof(CoreClientChunkDocument));

        return response?.Data;
    }
}

