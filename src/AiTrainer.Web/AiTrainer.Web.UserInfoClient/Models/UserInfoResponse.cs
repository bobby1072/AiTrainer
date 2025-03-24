namespace AiTrainer.Web.UserInfoClient.Models
{
    public record UserInfoResponse
    {
        public required string Email { get; init; }
        public required string Name { get; init; }
    }
}
