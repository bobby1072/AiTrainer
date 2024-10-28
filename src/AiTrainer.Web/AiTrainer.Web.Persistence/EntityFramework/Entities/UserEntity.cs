using AiTrainer.Web.Common.Models.DomainModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiTrainer.Web.Persistence.EntityFramework.Entities
{
    [Table("user", Schema = DbConstants.PublicSchema)]
    public record UserEntity : BaseEntity<Guid, User>
    {
        public string Email { get; set; }
        public string? Name { get; set; }
        public string Username { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public override User ToModel() => new(Email, Username, DateCreated, DateModified, Name, Id);
    }
}
