namespace AiTrainer.Web.Api.Models
{
    public record Outcome
    {
        public required bool IsSuccess { get; init; }
        public string? ExceptionMessage { get; init; }
    }

    public record Outcome<T> : Outcome
    {
        public T? Data { get; init; }
    }
}
