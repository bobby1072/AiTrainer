namespace AiTrainer.Web.Domain.Models.ApiModels.Request
{
    public sealed record OptionalIdInput
    {
        public Guid? Id { get; init; }
    }
}
