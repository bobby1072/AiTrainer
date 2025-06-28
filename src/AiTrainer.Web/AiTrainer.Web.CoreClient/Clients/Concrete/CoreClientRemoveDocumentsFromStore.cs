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

public class CoreClientRemoveDocumentsFromStore
    : ICoreClient<CoreRemoveDocumentsFromStoreInput, CoreFaissStoreResponse>
{
    private readonly ILogger<CoreClientRemoveDocumentsFromStore> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;

    public CoreClientRemoveDocumentsFromStore(
        ILogger<CoreClientRemoveDocumentsFromStore> logger,
        IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfiguration,
        IHttpContextAccessor httpContextAccessor,
        HttpClient httpClient
    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfiguration.Value;
        _httpContextAccessor = httpContextAccessor;
        _httpClient = httpClient;
    }

    public async Task<CoreFaissStoreResponse?> TryInvokeAsync(
        CoreRemoveDocumentsFromStoreInput input,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var correlationId = _httpContextAccessor.HttpContext.GetCorrelationId();

            var fileContent = new ByteArrayContent(input.FileInput);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                MediaTypeNames.Application.Octet
            );
            
            var response = await _aiTrainerCoreConfiguration.BaseEndpoint
                .AppendPathSegment("api")
                .AppendPathSegment("faissrouter")
                .AppendPathSegment("removedocuments")
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
                    ApiConstants.DefaultCamelCaseSerializerOptions, cancellationToken);

            return response?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Exception occured in {OpName} with message {Message}",
                nameof(CoreClientRemoveDocumentsFromStore),
                ex.Message
            );

            return null;
        }
    }
}
