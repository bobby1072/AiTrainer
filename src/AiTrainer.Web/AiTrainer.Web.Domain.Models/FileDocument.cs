using AiTrainer.Web.Domain.Models.Attributes;

namespace AiTrainer.Web.Domain.Models
{
    [LockedData]
    public class FileDocument : PersistableDomainModel<FileDocument, Guid?>
    {
        public Guid? CollectionId { get; set; }

        public required Guid UserId { get; set; }

        public required FileTypeEnum FileType { get; set; }

        public required string FileName { get; set; }

        public required byte[] FileData { get; set; }

        public string? FileDescription { get; set; }

        public bool FaissSynced { get; set; } = false;

        public required DateTime DateCreated { get; set; }

        public FileDocumentMetaData? MetaData { get; init; }
        public override bool Equals(FileDocument? other)
        {
            return Id == other?.Id
                && CollectionId == other?.CollectionId
                && DateCreated == other?.DateCreated
                && FileType == other.FileType
                && FileName == other.FileName
                && FileDescription == other.FileDescription
                && FaissSynced == other.FaissSynced
                && UserId == other.UserId;
        }

        public override void ApplyCreationDefaults()
        {
            Id = Guid.NewGuid();
            DateCreated = DateTime.UtcNow;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
