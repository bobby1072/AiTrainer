namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public sealed record PotentialDocumentEditChatRawQueryInput
{
    public required Guid FileDocumentId { get; init; }
    public required string ChangeRequest { get; init; }
}