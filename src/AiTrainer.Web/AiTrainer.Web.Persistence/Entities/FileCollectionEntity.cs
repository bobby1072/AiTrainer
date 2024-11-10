using AiTrainer.Web.Domain.Models;
using BT.Common.FastArray.Proto;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiTrainer.Web.Persistence.Entities
{
    public record FileCollectionEntity : BaseEntity<Guid, FileCollection>
    {
        public required Guid UserId { get; set; }

        public required string Name { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }
        public Guid? ParentId { get; set; }

        [ForeignKey(nameof(ParentId))]
        public FileCollectionEntity? Parent { get; set; }
        public virtual IReadOnlyCollection<FileCollectionEntity>? Children { get; set; }

        public override FileCollection ToModel() =>
            new FileCollection
            {
                Id = Id,
                UserId = UserId,
                Name = Name,
                DateCreated = DateCreated,
                DateModified = DateModified,
                Parent = Parent?.ToModel(),
                Children = Children?.FastArraySelect(x => x.ToModel()).ToArray(),
            };
    }
}
