using System.ComponentModel.DataAnnotations.Schema;
using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.Persistence.Entities;

[Table("file_document_metadata", Schema = DbConstants.PublicSchema)]
public record FileDocumentMetaDataEntity : BaseEntity<long, FileDocumentMetaData>
{
    public Guid DocumentId { get; init; }
    public string? Title { get; init; }
    public string? Author { get; init; }
    public string? Subject { get; init; }
    public string? Keywords { get; init; }
    public string? Creator { get; init; }
    public string? Producer { get; init; }
    public string? CreationDate { get; init; }
    public string? ModifiedDate { get; init; }
    public int? NumberOfPages { get; init; }
    public bool? IsEncrypted { get; init; }
    public Dictionary<string, object> ExtraData { get; init; } = [];

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
            ModifiedDate = ModifiedDate,
            NumberOfPages = NumberOfPages,
            IsEncrypted = IsEncrypted,
            ExtraData = ExtraData,
        };
    }
}
