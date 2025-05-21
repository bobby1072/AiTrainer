using AiTrainer.Web.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using BT.Common.FastArray.Proto;

namespace AiTrainer.Web.Persistence.Entities
{
    [Table("user", Schema = DbConstants.PublicSchema)]
    public sealed record UserEntity : BaseEntity<Guid, User>
    {
        public required string Email { get; set; }
        public string? Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public GlobalFileCollectionConfigEntity? GlobalFileCollectionConfig { get; init; }
        public override User ToModel() =>
            new()
            {
                Email = Email,
                DateCreated = DateCreated,
                DateModified = DateModified,
                Name = Name,
                Id = Id,
                GlobalFileCollectionConfig = GlobalFileCollectionConfig?.ToModel(),
            };
    }
}
