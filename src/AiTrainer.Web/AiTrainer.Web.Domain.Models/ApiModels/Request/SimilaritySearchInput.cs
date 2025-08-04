namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public sealed record SimilaritySearchInput
{
    public Guid? CollectionId { get; init; }

    public required int DocumentsToReturn { get; init; }

    public required string Question { get; init; }
}
