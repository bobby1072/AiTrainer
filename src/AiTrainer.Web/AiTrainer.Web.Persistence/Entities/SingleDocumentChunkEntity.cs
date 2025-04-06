using System.ComponentModel.DataAnnotations.Schema;
using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.Persistence.Entities;

[Table("single_document_chunks", Schema = DbConstants.PublicSchema)]
public record SingleDocumentChunkEntity: BaseEntity<Guid, SingleDocumentChunk>
{
    public required string PageContent { get; set; }
    public required Guid FileDocumentId { get; set; }
    public Dictionary<string, string>? MetaData { get; set; }

    public override SingleDocumentChunk ToModel()
    {
        return new SingleDocumentChunk
        {
            PageContent = PageContent,
            FaissId = FileDocumentId,
            MetaData = MetaData,
            Id = Id
        };
    }
}