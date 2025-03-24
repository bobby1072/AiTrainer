using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.UserInfoClient.Clients.Abstract;
using AiTrainer.Web.UserInfoClient.Models;
using BT.Common.Http.Extensions;
using BT.Common.OperationTimer.Proto;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace AiTrainer.Web.UserInfoClient.Clients.Concrete
{
    internal class UserInfoClient : IUserInfoClient
    {
        private readonly string _userInfoEndpoint;
        private readonly ILogger<UserInfoClient> _logger;
        private readonly UserInfoClientConfiguration _userInfoClientConfiguration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISerializer _serializer;
        public UserInfoClient(
            IOptionsSnapshot<ClientSettingsConfiguration> options,
            IHttpContextAccessor httpContextAccessor,
            IOptionsSnapshot<UserInfoClientConfiguration> userInfoClientConfiguration,
            ILogger<UserInfoClient> logger,
            ISerializer serializer
        )
        {
            _userInfoEndpoint = options.Value.UserInfoEndpoint;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _userInfoClientConfiguration = userInfoClientConfiguration.Value;
            _serializer = serializer;
        }

        public async Task<UserInfoResponse?> TryInvokeAsync(string accessToken)
        {
            try
            {
                var (timeTaken, results) = await OperationTimerUtils.TimeWithResultsAsync(() => InvokeAsync(accessToken));
                
                _logger.LogDebug("It took {TimeTaken}ms for the userinfo request to complete for correlationId {CorrelationId}",
                    timeTaken,
                    _httpContextAccessor.HttpContext?.GetCorrelationId());
                
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
            
            var response = await _userInfoEndpoint
                .WithHeader(HeaderNames.Authorization, accessToken.Split("Bearer ").Length == 2
                    ? accessToken
                    : $"Bearer {accessToken}")
                .WithSerializer(_serializer)
                .GetJsonAsync<UserInfoResponse>(_userInfoClientConfiguration);
            

            return response ?? throw new Exception("User info client returned null response");
        }
    }
}
