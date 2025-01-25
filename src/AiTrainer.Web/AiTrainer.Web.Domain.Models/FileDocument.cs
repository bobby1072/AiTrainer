using AiTrainer.Web.Domain.Models.Attributes;

namespace AiTrainer.Web.Domain.Models
{
    public record FileDocument : DomainModel<FileDocument, Guid?>
    {
        public Guid? CollectionId { get; set; }

        [LockedData]
        public required Guid UserId { get; set; }

        [LockedData]
        public required FileTypeEnum FileType { get; set; }

        [LockedData]
        public required string FileName { get; set; }

        [LockedData]
        public required byte[] FileData { get; set; }

        [LockedData]
        public string? FileDescription { get; set; }

        [LockedData]
        public required DateTime DateCreated { get; set; }

        public override bool Equals(FileDocument? other)
        {
            return Id == other?.Id
                && CollectionId == other?.CollectionId
                && DateCreated == other?.DateCreated
                && FileType == other.FileType
                && FileName == other.FileName
                && FileDescription == other.FileDescription
                && FileData == other.FileData
                && UserId == other.UserId;
        }

        public override void ApplyCreationDefaults()
        {
            DateCreated = DateTime.UtcNow;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
