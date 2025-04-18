using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
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

internal class CoreClientUpdateFaissStore: ICoreClient<CoreUpdateFaissStoreInput, CoreFaissStoreResponse>
{
    private readonly ILogger<CoreClientUpdateFaissStore> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CoreClientUpdateFaissStore(
        ILogger<CoreClientUpdateFaissStore> logger,
        IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfig,
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor; 
    }

    public async Task<CoreFaissStoreResponse?> TryInvokeAsync(CoreUpdateFaissStoreInput input, CancellationToken cancellation = default)
    {
        try
        {
            var correlationId = _httpContextAccessor.HttpContext.GetCorrelationId();

            using var fileContent = new ByteArrayContent(input.FileInput);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Octet);
            
            using var formContent = new MultipartFormDataContent();
            formContent.Add(fileContent, "file", "docStore.index");
            formContent.Add(new StringContent(JsonSerializer.Serialize(input, ApiConstants.DefaultCamelCaseSerializerOptions),
                    Encoding.UTF8,
                    "application/json"),
                "metadata");
            
            using var httpResult = await _httpClient
                .SendWithRetry(
                    requestMessage =>
                    {
                        requestMessage.Method = HttpMethod.Post;
                        requestMessage.RequestUri = new Uri($"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/faissrouter/updatestore");
                        requestMessage.Headers.AddApiKeyHeader(_aiTrainerCoreConfiguration.ApiKey);
                        requestMessage.Headers.AddCorrelationIdHeader(_httpContextAccessor.HttpContext.GetCorrelationId());
                        
                        requestMessage.Content = formContent;
                    },
                    _aiTrainerCoreConfiguration,
                    _logger,
                    nameof(CoreClientUpdateFaissStore),
                    correlationId?.ToString(),
                    cancellation
                );

            var result = await httpResult.Content
                .TryDeserializeJson<CoreResponse<CoreFaissStoreResponse>>(
                    ApiConstants.DefaultCamelCaseSerializerOptions, cancellation);
            
            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occured in {OpName} with message {Message}",
                nameof(CoreClientUpdateFaissStore),
                ex.Message);
            
            return null;
        }
    }
}