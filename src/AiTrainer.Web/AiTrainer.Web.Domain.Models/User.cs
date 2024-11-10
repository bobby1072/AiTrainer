using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models
{
    public class User : DomainModel<User, Guid?>
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("dateCreated")]
        public required DateTime DateCreated { get; set; }

        [JsonPropertyName("dateModified")]
        public required DateTime DateModified { get; set; }

        public override bool Equals(DomainModel<User, Guid?>? other)
        {
            return other is User user
                && Id == user.Id
                && Email == user.Email
                && Name == user.Name
                && DateCreated == user.DateCreated
                && DateModified == user.DateModified;
        }

        public override void ApplyCreationDefaults()
        {
            DateCreated = DateTime.UtcNow;
            DateModified = DateTime.UtcNow;
        }

        public static string GetCacheKey(string accessToken) => $"{_cacheKey}{accessToken}";

        private const string _cacheKey = "cacheUser-";
    }
}
