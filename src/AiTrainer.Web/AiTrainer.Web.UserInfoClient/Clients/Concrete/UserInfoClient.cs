using System.Net.Http.Json;
using System.Text.Json;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.UserInfoClient.Clients.Abstract;
using AiTrainer.Web.UserInfoClient.Models;
using BT.Common.OperationTimer.Proto;
using BT.Common.Polly.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace AiTrainer.Web.UserInfoClient.Clients.Concrete
{
    internal class UserInfoClient : IUserInfoClient
    {
        private readonly ILogger<UserInfoClient> _logger;
        private readonly string _userInfoEndpoint;
        private readonly UserInfoClientConfiguration _userInfoClientConfiguration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        public UserInfoClient(
            IOptions<ClientSettingsConfiguration> options,
            IHttpContextAccessor httpContextAccessor,
            IOptions<UserInfoClientConfiguration> userInfoClientConfiguration,
            ILogger<UserInfoClient> logger,
            HttpClient httpClient
        )
        {
            _userInfoEndpoint = options.Value.InternalUserInfoEndpoint;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _userInfoClientConfiguration = userInfoClientConfiguration.Value;
            _httpClient = httpClient;
        }

        public async Task<UserInfoResponse?> TryInvokeAsync(string accessToken)
        {
            try
            {
                var retryPipeline = _userInfoClientConfiguration.ToPipeline();
                var (timeTaken, results) = await OperationTimerUtils.TimeWithResultsAsync(
                    () => retryPipeline.ExecuteAsync(async ct => await InvokeAsync(accessToken))
                );

                _logger.LogDebug(
                    "It took {TimeTaken}ms for the userinfo request to complete for correlationId {CorrelationId}",
                    timeTaken,
                    _httpContextAccessor.HttpContext?.GetCorrelationId()
                );

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "User info client threw exception with message {Message}",
                    ex.Message
                );

                return null;
            }
        }

        private async Task<UserInfoResponse> InvokeAsync(string accessToken)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, _userInfoEndpoint);
            requestMessage.Headers.TryAddWithoutValidation(
                HeaderNames.Authorization,
                $"Bearer {accessToken}"
            );

            using var httpResult = await _httpClient.SendAsync(requestMessage);

            var finalResult = await httpResult.Content.ReadFromJsonAsync<UserInfoResponse>();

            return finalResult
                ?? throw new JsonException("User info client returned null response");
        }
    }
}
