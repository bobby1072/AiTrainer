namespace AiTrainer.Web.Api.Models
{
    public record Outcome
    {
        public bool IsSuccess => ExceptionMessage is null;
        public string? ExceptionMessage { get; init; }
    }

    public record Outcome<T> : Outcome
    {
        public T? Data { get; init; }
    }
}
