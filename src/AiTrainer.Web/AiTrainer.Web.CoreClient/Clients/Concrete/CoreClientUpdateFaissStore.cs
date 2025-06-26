using System.Net.Http.Headers;
using System.Net.Mime;
using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.Http.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;

internal class CoreClientUpdateFaissStore
    : ICoreClient<CoreUpdateFaissStoreInput, CoreFaissStoreResponse>
{
    private readonly ILogger<CoreClientUpdateFaissStore> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CoreClientUpdateFaissStore(
        ILogger<CoreClientUpdateFaissStore> logger,
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

    public async Task<CoreFaissStoreResponse?> TryInvokeAsync(
        CoreUpdateFaissStoreInput input,
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
            

            var result = await _aiTrainerCoreConfiguration.BaseEndpoint
                .AppendPathSegment("api")
                .AppendPathSegment("faissrouter")
                .AppendPathSegment("updatestore")
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
                .PostJsonAsync<CoreResponse<CoreFaissStoreResponse>>(_httpClient,
                    ApiConstants.DefaultCamelCaseSerializerOptions, cancellation);
            

            return result?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Exception occured in {OpName} with message {Message}",
                nameof(CoreClientUpdateFaissStore),
                ex.Message
            );

            return null;
        }
    }
}
