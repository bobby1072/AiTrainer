using BT.Common.Helpers.Extensions;

namespace AiTrainer.Web.Domain.Models;

public class SingleDocumentChunk : PersistableDomainModel<SingleDocumentChunk, Guid?>
{
    public required string PageContent { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
    public required Guid FileDocumentId { get; set; }

    public override bool Equals(SingleDocumentChunk? obj)
    {
        return PageContent == obj?.PageContent
            && FileDocumentId == obj.FileDocumentId
            && Metadata.IsStringSequenceEqual(obj.Metadata);
    }
}
