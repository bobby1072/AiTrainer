using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.UserInfoClient.Clients.Abstract;
using AiTrainer.Web.UserInfoClient.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;

namespace AiTrainer.Web.UserInfoClient.Clients.Concrete
{
    internal class UserInfoClient: IUserInfoClient
    {
        private readonly string _userInfoEndpoint;
        private HttpClient _client;
        public UserInfoClient(HttpClient httpClient, IOptions<ClientSettingsConfiguration> options)
        {
            _userInfoEndpoint = options.Value.UserInfoEndpoint;
            _client = httpClient;
        }
        public async Task<UserInfoResponse> InvokeAsync(string accessToken)
        {
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_userInfoEndpoint),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), accessToken.Split("Bearer ").Length == 2 ? accessToken : $"Bearer {accessToken}"},
                }
            };

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var jsonContent = await response.Content.ReadFromJsonAsync<UserInfoResponse>();

            throw new NotImplementedException();

        }
    }
}