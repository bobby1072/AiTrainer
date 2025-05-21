using AiTrainer.Web.Domain.Models;
using BT.Common.FastArray.Proto;
using System.ComponentModel.DataAnnotations.Schema;

namespace AiTrainer.Web.Persistence.Entities
{
    [Table("file_collection", Schema = DbConstants.PublicSchema)]
    public sealed record FileCollectionEntity : BaseEntity<Guid, FileCollection>
    {
        public required Guid UserId { get; set; }
        public required string CollectionName { get; set; }
        public string? CollectionDescription { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Guid? ParentId { get; set; }
        public bool AutoFaissSync { get; set; }
        public IReadOnlyCollection<FileDocumentEntity>? Documents { get; set; }
        public FileCollectionFaissEntity? FaissStore { get; set; }
        public IReadOnlyCollection<SharedFileCollectionMemberEntity>? SharedFileMembers { get; set; }
        public override FileCollection ToModel() =>
            new()
            {
                Id = Id,
                UserId = UserId,
                CollectionName = CollectionName,
                AutoFaissSync = AutoFaissSync,
                DateCreated = DateCreated,
                DateModified = DateModified,
                ParentId = ParentId,
                CollectionDescription = CollectionDescription,
                FaissStore = FaissStore?.ToModel(),
                Documents = Documents?.FastArraySelect(x => x.ToModel()).ToArray(),
                SharedFileMembers = SharedFileMembers?.FastArraySelect(x => x.ToModel()).ToArray(),
            };
    }
}
