using System.Net;
using System.Net.Http.Json;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.UserInfoClient.Clients.Abstract;
using AiTrainer.Web.UserInfoClient.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.UserInfoClient.Clients.Concrete
{
    internal class UserInfoClient : IUserInfoClient
    {
        private readonly string _userInfoEndpoint;
        private HttpClient _client;
        private readonly ILogger<UserInfoClient> _logger;

        public UserInfoClient(
            HttpClient httpClient,
            IOptions<ClientSettingsConfiguration> options,
            ILogger<UserInfoClient> logger
        )
        {
            _userInfoEndpoint = options.Value.UserInfoEndpoint;
            _client = httpClient;
            _logger = logger;
        }

        public async Task<UserInfoResponse?> TryInvokeAsync(string accessToken)
        {
            try
            {
                return await InvokeAsync(accessToken);
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

        public async Task<UserInfoResponse> InvokeAsync(string accessToken)
        {
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_userInfoEndpoint),
                Headers =
                {
                    {
                        HttpRequestHeader.Authorization.ToString(),
                        accessToken.Split("Bearer ").Length == 2
                            ? accessToken
                            : $"Bearer {accessToken}"
                    },
                },
            };

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadFromJsonAsync<UserInfoResponse>();

            return jsonContent ?? throw new Exception("User info client returned null response");
        }
    }
}
