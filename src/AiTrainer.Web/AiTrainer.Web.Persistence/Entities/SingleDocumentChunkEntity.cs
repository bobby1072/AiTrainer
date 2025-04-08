using System.ComponentModel.DataAnnotations.Schema;
using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.Persistence.Entities;

[Table("single_document_chunk", Schema = DbConstants.PublicSchema)]
public record SingleDocumentChunkEntity : BaseEntity<Guid, SingleDocumentChunk>
{
    public required string PageContent { get; set; }
    public required Guid FileDocumentId { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }

    public override SingleDocumentChunk ToModel()
    {
        return new SingleDocumentChunk
        {
            PageContent = PageContent,
            FileDocumentId = FileDocumentId,
            Metadata = Metadata,
            Id = Id
        };
    }
}