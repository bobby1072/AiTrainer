using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using AiTrainer.Web.Domain.Models;
using BT.Common.FastArray.Proto;

namespace AiTrainer.Web.Persistence.Entities
{
    public record FileCollectionEntity : BaseEntity<Guid, FileCollection>
    {
        public required Guid UserId { get; set; }

        public required string CollectionName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }
        public Guid? ParentId { get; set; }

        [ForeignKey(nameof(ParentId))]
        public FileCollectionEntity? Parent { get; set; }
        public virtual IReadOnlyCollection<FileCollectionEntity>? Children { get; set; }
        public required byte[] FaissIndex { get; set; }
        public required JsonDocument FaissJson { get; set; }

        public override FileCollection ToModel() =>
            new FileCollection
            {
                Id = Id,
                UserId = UserId,
                CollectionName = CollectionName,
                DateCreated = DateCreated,
                DateModified = DateModified,
                FaissIndex = FaissIndex,
                FaissJson = FaissJson,
                ParentId = ParentId,
                Parent = Parent?.ToModel(),
                Children = Children?.FastArraySelect(x => x.ToModel()).ToArray(),
            };
    }
}
