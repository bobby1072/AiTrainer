using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.Persistence.Entities;

public record FileDocumentMetaDataEntity : BaseEntity<long, FileDocumentMetaData>
{
    public required Guid DocumentId { get; init; }
    public string? Title { get; init; }
    public string? Author { get; init; }
    public string? Subject { get; init; }
    public string? Keywords { get; init; }
    public string? Creator { get; init; }
    public string? Producer { get; init; }
    public string? CreationDate { get; init; }
    public string? ModificationDate { get; init; }
    public int? NumberOfPages { get; init; }
    public bool IsEncrypted { get; init; }
    public Dictionary<string, object>? ExtraData { get; init; }

    public override FileDocumentMetaData ToModel()
    {
        return new FileDocumentMetaData
        {
            Id = Id,
            DocumentId = DocumentId,
            Title = Title,
            Author = Author,
            Subject = Subject,
            Keywords = Keywords,
            Creator = Creator,
            Producer = Producer,
            CreationDate = CreationDate,
            ModificationDate = ModificationDate,
            NumberOfPages = NumberOfPages,
            IsEncrypted = IsEncrypted,
            ExtraData = ExtraData,
        };
    }
}
