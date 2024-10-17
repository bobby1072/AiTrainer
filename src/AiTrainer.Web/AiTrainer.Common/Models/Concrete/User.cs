using System.Text.Json.Serialization;
using AiTrainer.Web.Common.Models.Abstract;

namespace AiTrainer.Web.Common.Models.Concrete
{
    public class User : RuntimeBase, IEquatable<User>
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        public User(string email, string username, string? name = null, Guid? id = null)
        {
            Id = id;
            Email = email;
            Name = name;
            Username = username;
        }

        public override bool Equals(RuntimeBase? other)
        {
            return other is User user
                && Id == user.Id
                && Email == user.Email
                && Name == user.Name
                && Username == user.Username;
        }

        public bool Equals(User? obj) => Equals(obj);
    }
}
