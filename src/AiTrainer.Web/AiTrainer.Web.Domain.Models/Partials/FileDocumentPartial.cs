using AiTrainer.Web.Domain.Models.Partials;

namespace AiTrainer.Web.Domain.Models
{
    public record FileDocumentPartial : DomainModelPartial<FileDocumentPartial, long?>
    {
        public required Guid CollectionId { get; init; }
        public required Guid UserId { get; init; }
        public required FileTypeEnum FileType { get; init; }
        public required string FileName { get; init; }
        public required DateTime DateCreated { get; init; }
        public override bool Equals(FileDocumentPartial? other)
        {
            return other is FileDocumentPartial fileDocumentPartial
                && Id == fileDocumentPartial.Id
                && CollectionId == fileDocumentPartial.CollectionId
                && DateCreated == fileDocumentPartial.DateCreated
                && FileType == fileDocumentPartial.FileType
                && FileName == fileDocumentPartial.FileName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}