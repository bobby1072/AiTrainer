using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models
{
    public class User : DomainModel<Guid?>, IEquatable<User>
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("username")]
        public required string Username { get; set; }

        [JsonPropertyName("dateCreated")]
        public required DateTime DateCreated { get; set; }

        [JsonPropertyName("dateModified")]
        public required DateTime DateModified { get; set; }

        public override bool Equals(DomainModel<Guid?>? other)
        {
            return other is User user
                && Id == user.Id
                && Email == user.Email
                && Name == user.Name
                && Username == user.Username
                && DateCreated == user.DateCreated
                && DateModified == user.DateModified;
        }

        public override void ApplyCreationDefaults()
        {
            DateCreated = DateTime.UtcNow;
            DateModified = DateTime.UtcNow;
        }

        public bool Equals(User? obj) => Equals((DomainModel<Guid?>?)obj);

        public static string GetCacheKey(string accessToken) => $"{_cacheKey}{accessToken}";

        private const string _cacheKey = "cacheUser-";
    }
}
