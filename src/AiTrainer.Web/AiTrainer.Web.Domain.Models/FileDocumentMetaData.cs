using AiTrainer.Web.Domain.Models.Attributes;

namespace AiTrainer.Web.Domain.Models;

[LockedData]
public record FileDocumentMetaData : DomainModel<FileDocumentMetaData, long?>
{
    public required Guid DocumentId { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Subject { get; set; }
    public string? Keywords { get; set; }
    public string? Creator { get; set; }
    public string? Producer { get; set; }
    public string? CreationDate { get; set; }
    public string? ModifiedDate { get; set; }
    public int? NumberOfPages { get; set; }
    public bool? IsEncrypted { get; set; }
    public Dictionary<string, string?> ExtraData { get; set; } = [];

    public override bool Equals(FileDocumentMetaData? obj)
    {
        return Id == obj?.Id
            && Title == obj?.Title
            && Author == obj?.Author
            && Subject == obj?.Subject
            && Keywords == obj?.Keywords
            && Creator == obj?.Creator
            && Producer == obj?.Producer
            && CreationDate == obj?.CreationDate
            && ModifiedDate == obj?.ModifiedDate;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
