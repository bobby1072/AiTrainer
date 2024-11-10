using System.ComponentModel.DataAnnotations.Schema;
using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.Persistence.Entities
{
    [Table("user", Schema = DbConstants.PublicSchema)]
    public record UserEntity : BaseEntity<Guid, User>
    {
        public required string Email { get; set; }
        public string? Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public override User ToModel() =>
            new User
            {
                Email = Email,
                DateCreated = DateCreated,
                DateModified = DateModified,
                Name = Name,
                Id = Id,
            };
    }
}
