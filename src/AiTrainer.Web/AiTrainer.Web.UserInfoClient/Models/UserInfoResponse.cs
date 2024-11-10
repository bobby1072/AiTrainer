using System.Text.Json.Serialization;

namespace AiTrainer.Web.UserInfoClient.Models
{
    public class UserInfoResponse
    {
        [JsonPropertyName("email")]
        public required string Email { get; init; }

        [JsonPropertyName("name")]
        public required string Name { get; init; }
    }
}
