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
using BT.Common.Http.Extensions;
using BT.Common.Polly.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;

public class CoreClientSimilaritySearch
    : ICoreClient<CoreSimilaritySearchInput, CoreSimilaritySearchResponse>
{
    private readonly ILogger<CoreClientSimilaritySearch> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;

    public CoreClientSimilaritySearch(
        ILogger<CoreClientSimilaritySearch> logger,
        IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfig,
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CoreSimilaritySearchResponse?> TryInvokeAsync(
        CoreSimilaritySearchInput input,
        CancellationToken cancellation = default
    )
    {
        try
        {
            var correlationId = _httpContextAccessor.HttpContext.GetCorrelationId();
            
            var fileContent = new ByteArrayContent(input.FileInput);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                MediaTypeNames.Application.Octet
            );

            var pipeline = _aiTrainerCoreConfiguration.ToPipeline();
            
            var response = await pipeline.ExecuteAsync(async ct => await _aiTrainerCoreConfiguration.BaseEndpoint
                .AppendPathSegment("api")
                .AppendPathSegment("faissrouter")
                .AppendPathSegment("similaritysearch")
                .WithCoreApiKeyHeader(_aiTrainerCoreConfiguration.ApiKey)
                .WithCorrelationIdHeader(correlationId?.ToString())
                .WithMultipartFormData(x =>
                {
                    x.Add(fileContent, "file", "docStore.index");
                    x.Add(
                        CoreClientHttpExtensions.CreateApplicationJson(
                            input,
                            ApiConstants.DefaultCamelCaseSerializerOptions
                        ),
                        "metadata"
                    );
                })
                .PostJsonAsync<CoreResponse<CoreSimilaritySearchResponse>>(_httpClient,
                    ApiConstants.DefaultCamelCaseSerializerOptions, ct), cancellation);
            

            return response?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Exception occured in {OpName} with message {Message}",
                nameof(CoreClientSimilaritySearch),
                ex.Message
            );

            return null;
        }
    }
}
