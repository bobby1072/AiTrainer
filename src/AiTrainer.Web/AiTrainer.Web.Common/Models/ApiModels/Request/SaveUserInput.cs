namespace AiTrainer.Web.Common.Models.ApiModels.Request
{
    public record SaveUserInput
    {
        public Guid? NewDeviceToken { get; init; }
        public required string Email { get; init; }
        public required string Name { get; init; }
    }
}
