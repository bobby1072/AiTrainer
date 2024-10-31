using System.Text.Json.Serialization;

namespace AiTrainer.Web.UserInfoClient.Models
{
    public sealed class UserInfoResponse
    {
        [JsonPropertyName("email")]
        public string Email { get; init; }
        [JsonPropertyName("name")]
        public string Name { get; init; }
    }
}
