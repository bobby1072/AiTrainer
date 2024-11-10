namespace AiTrainer.Web.Domain.Models
{
    public record FileDocument : DomainModel<FileDocument, Guid?>
    {
        public required Guid CollectionId { get; set; }
        public required FileTypeEnum FileType { get; set; }
        public required string FileName { get; set; }
        public required string FileData { get; set; }
        public required DateTime DateCreated { get; set; }

        public override bool Equals(FileDocument? other)
        {
            return other is FileDocument fileDocument
                && Id == fileDocument.Id
                && CollectionId == fileDocument.CollectionId
                && DateCreated == fileDocument.DateCreated;
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
