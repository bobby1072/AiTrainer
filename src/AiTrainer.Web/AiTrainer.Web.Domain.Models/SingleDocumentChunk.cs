using BT.Common.Helpers.Extensions;

namespace AiTrainer.Web.Domain.Models;

public class SingleDocumentChunk : DomainModel<SingleDocumentChunk>
{
    public required Guid Id { get; set; }
    public required string PageContent { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
    public required Guid FileDocumentId { get; set; }

    public override bool Equals(SingleDocumentChunk? obj)
    {
        return Id == obj?.Id && 
            PageContent == obj?.PageContent
            && FileDocumentId == obj.FileDocumentId
            && Metadata.IsStringSequenceEqual(obj.Metadata);
    }
}
