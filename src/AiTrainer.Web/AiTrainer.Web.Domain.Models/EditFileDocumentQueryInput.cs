
namespace AiTrainer.Web.Domain.Models;

public sealed record EditFileDocumentQueryInput: ChatQueryInput
{
    public required FileDocument FileDocumentToChange { get; init; }
    public required string ChangeRequest { get; init; }
}