using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models
{
    public class FileCollection : DomainModel<FileCollection, Guid?>
    {
        [JsonPropertyName("userId")]
        public required Guid UserId { get; set; }

        [JsonPropertyName("user")]
        public User? User { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("dateCreated")]
        public required DateTime DateCreated { get; set; }

        [JsonPropertyName("dateModified")]
        public required DateTime DateModified { get; set; }

        public override bool Equals(DomainModel<FileCollection, Guid?>? other)
        {
            return other is FileCollection fileCollection
                && Id == fileCollection.Id
                && UserId == fileCollection.UserId
                && Name == fileCollection.Name
                && DateCreated == fileCollection.DateCreated
                && DateModified == fileCollection.DateModified;
        }

        public override void ApplyCreationDefaults()
        {
            DateModified = DateTime.UtcNow;
            DateCreated = DateTime.UtcNow;
        }
    }
}
