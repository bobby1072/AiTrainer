using BT.Common.Helpers.Extensions;

namespace AiTrainer.Web.Domain.Models;

public class SingleInnerFaissJsonDocFileContent: DomainModel<SingleInnerFaissJsonDocFileContent>
{
    public Dictionary<string, string>? Metadata { get; init; }
    public required string PageContent { get; init; }

    public override bool Equals(SingleInnerFaissJsonDocFileContent? obj)
    {
        return obj?.PageContent == PageContent &&
               obj?.Metadata.IsStringSequenceEqual(Metadata) == true;
    }
}