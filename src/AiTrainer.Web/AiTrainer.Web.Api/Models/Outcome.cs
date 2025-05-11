namespace AiTrainer.Web.Api.Models
{
    public record Outcome
    {
        public bool IsSuccess => string.IsNullOrEmpty(ExceptionMessage);
        public string? ExceptionMessage { get; init; }
    }

    public sealed record Outcome<T> : Outcome
    {
        public T? Data { get; init; }
    }
}
