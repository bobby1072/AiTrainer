namespace AiTrainer.Web.Domain.Models.Partials
{
    public record FileDocumentPartial : DomainModelPartial<FileDocumentPartial, Guid?>
    {
        public Guid? CollectionId { get; init; }
        public required Guid UserId { get; init; }
        public required FileTypeEnum FileType { get; init; }
        public required string FileName { get; init; }
        public string? FileDescription { get; init; }
        public bool FaissSynced { get; init; }
        public required DateTime DateCreated { get; init; }
        public FileDocumentMetaData? MetaData { get; init; }

        public override bool Equals(FileDocumentPartial? other)
        {
            return other is FileDocumentPartial fileDocumentPartial
                && Id == fileDocumentPartial.Id
                && CollectionId == fileDocumentPartial.CollectionId
                && DateCreated == fileDocumentPartial.DateCreated
                && FileType == fileDocumentPartial.FileType
                && other.FaissSynced == fileDocumentPartial.FaissSynced
                && FileName == fileDocumentPartial.FileName
                && FileDescription == fileDocumentPartial.FileDescription;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
