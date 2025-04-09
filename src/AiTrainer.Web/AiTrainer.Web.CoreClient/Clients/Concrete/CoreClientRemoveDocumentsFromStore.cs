using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.Polly.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;

public class CoreClientRemoveDocumentsFromStore: ICoreClient<CoreRemoveDocumentsFromStoreInput, CoreFaissStoreResponse>
{
    private readonly ILogger<CoreClientRemoveDocumentsFromStore> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient; 
        
    public CoreClientRemoveDocumentsFromStore(
        ILogger<CoreClientRemoveDocumentsFromStore> logger,
        AiTrainerCoreConfiguration aiTrainerCoreConfiguration,
        IHttpContextAccessor httpContextAccessor,
        HttpClient httpClient
    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfiguration;
        _httpContextAccessor = httpContextAccessor;
        _httpClient = httpClient;
    }
    
    public async Task<CoreFaissStoreResponse?> TryInvokeAsync(CoreRemoveDocumentsFromStoreInput input, CancellationToken cancellationToken = default)
    {
        var metadata = new
        {
            jsonDocStore = input.DocStore,
            documentIdsToRemove = input.DocumentIdsToRemove,
        };
        
        var metadataJson = JsonSerializer.Serialize(metadata);
        
        using var content = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(input.FileInput);
        
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
        fileContent.Headers.AddApiKeyHeader(_aiTrainerCoreConfiguration.ApiKey);
        fileContent.Headers.AddCorrelationIdHeader(_httpContextAccessor.HttpContext.GetCorrelationId());
        
        content.Add(fileContent, "file", "upload.pdf"); 
        content.Add(new StringContent(metadataJson, Encoding.UTF8, "application/json"), "metadata");
        var retryPipeline = _aiTrainerCoreConfiguration.ToPipeline();
        var response = await retryPipeline.ExecuteAsync(async ct => await _httpClient.PostAsync($"{_aiTrainerCoreConfiguration.BaseEndpoint}/", content,
            ct), cancellationToken);

        response.EnsureSuccessStatusCode();

        var parsedResponse = await response.Content
            .ReadFromJsonAsync<CoreFaissStoreResponse>(cancellationToken)
            ?? throw new SerializationException("Failed to deserialize response");
        
        return parsedResponse;
    }
}