using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
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

            using var httpResult = await _httpClient.SendWithRetry(
                requestMessage =>
                {
                    requestMessage.Method = HttpMethod.Post;
                    requestMessage.RequestUri = new Uri(
                        $"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/openairouter/formattedchatquery"
                    );
                    requestMessage.Headers.AddApiKeyHeader(_aiTrainerCoreConfiguration.ApiKey);
                    requestMessage.Headers.AddCorrelationIdHeader(correlationId);

                    requestMessage.Content = CoreClientHttpExtensions.CreateApplicationJson(
                        request.ToCoreInput(),
                        ApiConstants.DefaultCamelCaseSerializerOptions
                    );
                },
                _aiTrainerCoreConfiguration,
                _logger,
                nameof(CoreClientFormattedChatQuery),
                correlationId?.ToString(),
                cancellationToken
            );

            var finalResult = await httpResult.Content.TryDeserializeJson<
                CoreResponse<CoreFormattedChatQueryResponse>
            >(ApiConstants.DefaultCamelCaseSerializerOptions, cancellationToken);

            return finalResult?.Data;
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
