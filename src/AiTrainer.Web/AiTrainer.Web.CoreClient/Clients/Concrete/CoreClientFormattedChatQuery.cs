using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using BT.Common.Http.Extensions;
using BT.Common.Polly.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;

internal class CoreClientFormattedChatQuery
    : ICoreClient<FormattedChatQueryBuilder, CoreFormattedChatQueryResponse>
{
    private readonly ILogger<CoreClientFormattedChatQuery> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;

    public CoreClientFormattedChatQuery(
        ILogger<CoreClientFormattedChatQuery> logger,
        IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfig,
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
        _httpContextAccessor = httpContextAccessor;
        _httpClient = httpClient;
    }

    public async Task<CoreFormattedChatQueryResponse?> TryInvokeAsync(
        FormattedChatQueryBuilder request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var correlationId = _httpContextAccessor.HttpContext.GetCorrelationId();

            var pipeline = _aiTrainerCoreConfiguration.ToPipeline();
            
            var response = await pipeline.ExecuteAsync(async ct => await _aiTrainerCoreConfiguration.BaseEndpoint
                .AppendPathSegment("api")
                .AppendPathSegment("openairouter")
                .AppendPathSegment("formattedchatquery")
                .WithCorrelationIdHeader(correlationId.ToString())
                .WithCoreApiKeyHeader(_aiTrainerCoreConfiguration.ApiKey)
                .WithApplicationJson(request.ToCoreInput(), ApiConstants.DefaultCamelCaseSerializerOptions)
                .PostJsonAsync<CoreResponse<CoreFormattedChatQueryResponse>>(_httpClient,
                    ApiConstants.DefaultCamelCaseSerializerOptions, ct), cancellationToken);

            return response?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Exception occured in {OpName} with message {Message}",
                nameof(CoreClientFormattedChatQuery),
                ex.Message
            );

            return null;
        }
    }
}
