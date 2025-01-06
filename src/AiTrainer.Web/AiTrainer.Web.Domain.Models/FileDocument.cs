using AiTrainer.Web.Domain.Models.Attributes;

namespace AiTrainer.Web.Domain.Models
{
    public record FileDocument : DomainModel<FileDocument, Guid?>
    {
        public Guid? CollectionId { get; set; }

        [LockedProperty]
        public required Guid UserId { get; set; }

        [LockedProperty]
        public required FileTypeEnum FileType { get; set; }

        [LockedProperty]
        public required string FileName { get; set; }

        [LockedProperty]
        public required byte[] FileData { get; set; }

        [LockedProperty]
        public string? FileDescription { get; set; }

        [LockedProperty]
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
