namespace AiTrainer.Web.Domain.Models.ApiModels.Response;

public sealed record ClientConfigurationResponse
{
    public required string Scope { get; init; }
    public required string AuthorityHost { get; init; }
    public required string AuthorityClientId { get; init; }
}