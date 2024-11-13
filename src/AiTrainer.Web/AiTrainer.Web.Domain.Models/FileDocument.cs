using AiTrainer.Web.Domain.Models.Attributes;

namespace AiTrainer.Web.Domain.Models
{
    public record FileDocument : DomainModel<FileDocument, Guid?>
    {
        [LockedProperty]
        public Guid? CollectionId { get; set; }
        [LockedProperty]
        public required FileTypeEnum FileType { get; set; }
        public required string FileName { get; set; }
        [LockedProperty]
        public required byte[] FileData { get; set; }
        [LockedProperty]
        public required DateTime DateCreated { get; set; }

        public override bool Equals(FileDocument? other)
        {
            return other is FileDocument fileDocument
                && Id == fileDocument.Id
                && CollectionId == fileDocument.CollectionId
                && DateCreated == fileDocument.DateCreated
                && FileType == fileDocument.FileType
                && FileName == fileDocument.FileName
                && FileData == fileDocument.FileData;
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
