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

public class CoreClientSimilaritySearch: ICoreClient<SimilaritySearchInput, SimilaritySearchResponse>
{
    private readonly ILogger<CoreClientSimilaritySearch> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CoreClientSimilaritySearch(
        ILogger<CoreClientSimilaritySearch> logger,
        IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfig,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<SimilaritySearchResponse?> TryInvokeAsync(SimilaritySearchInput input)
    {
        var response = await _aiTrainerCoreConfiguration.BaseEndpoint
            .AppendPathSegment("api")
            .AppendPathSegment("faissrouter")
            .AppendPathSegment("similaritysearch")
            .WithAiTrainerCoreKeyHeader(_aiTrainerCoreConfiguration.ApiKey)
            .WithCorrelationIdHeader(_httpContextAccessor.HttpContext.GetCorrelationId())
            .PostMultipartAsync(x =>
            {
                var indexFileStream = new MemoryStream(input.FileInput);
                var n = x.AddJson("metadata", input);
                x.AddFile("file", indexFileStream, "docStore.index");
            })
            .ReceiveJsonAsync<CoreResponse<SimilaritySearchResponse>>(_aiTrainerCoreConfiguration)
            .CoreClientExceptionHandling(_logger, nameof(FaissStoreResponse));
        
        
        return response?.Data;
    } 
}
