using System.Text.Json.Serialization;

namespace AiTrainer.Web.Common.Models.DomainModels
{
    public class User : DomainModel, IEquatable<User>
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("dateModified")]
        public DateTime DateModified { get; set; }

        public User(
            string email,
            string username,
            DateTime dateCreated,
            DateTime dateModified,
            string? name = null,
            Guid? id = null
        )
        {
            Id = id;
            Email = email;
            Name = name;
            Username = username;
            DateCreated = dateCreated;
            DateModified = dateModified;
        }

        public override bool Equals(DomainModel? other)
        {
            return other is User user
                && Id == user.Id
                && Email == user.Email
                && Name == user.Name
                && Username == user.Username
                && DateCreated == user.DateCreated
                && DateModified == user.DateModified;
        }

        public bool Equals(User? obj) => Equals(obj);
    }
}
