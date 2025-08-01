using AiTrainer.Web.Domain.Models.Attributes;

namespace AiTrainer.Web.Domain.Models
{
    public class FileCollection : PersistableDomainModel<FileCollection, Guid?>
    {
        [LockedData]
        public required Guid UserId { get; set; }
        public required string CollectionName { get; set; }
        public string? CollectionDescription { get; set; }

        [LockedData]
        public required DateTime DateCreated { get; set; }
        public required DateTime DateModified { get; set; }
        public Guid? ParentId { get; set; }
        public bool AutoFaissSync { get; init; }
        public FileCollectionFaiss? FaissStore { get; init; }
        public IReadOnlyCollection<FileDocument>? Documents { get; init; }
        
        public IReadOnlyCollection<SharedFileCollectionMember>? SharedFileCollectionMembers { get; init; }

        public override bool Equals(FileCollection? other)
        {
            return other is FileCollection fileCollection
                && Id == fileCollection.Id
                && UserId == fileCollection.UserId
                && CollectionName == fileCollection.CollectionName
                && DateCreated == fileCollection.DateCreated
                && DateModified == fileCollection.DateModified
                && ParentId == fileCollection.ParentId
                && CollectionDescription == fileCollection.CollectionDescription;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void ApplyCreationDefaults()
        {
            DateModified = DateTime.UtcNow;
            DateCreated = DateTime.UtcNow;
        }
    }
}
