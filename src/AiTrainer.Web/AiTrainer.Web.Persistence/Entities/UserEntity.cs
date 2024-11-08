using AiTrainer.Web.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiTrainer.Web.Persistence.Entities
{
    [Table("user", Schema = DbConstants.PublicSchema)]
    public record UserEntity : BaseEntity<Guid, User>
    {
        public required string Email { get; set; }
        public string? Name { get; set; }
        public required string Username { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public override User ToModel() => new(Email, Username, DateCreated, DateModified, Name, Id);
    }
}
