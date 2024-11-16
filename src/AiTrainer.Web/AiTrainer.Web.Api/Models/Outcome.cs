namespace AiTrainer.Web.Api.Models
{
    public class Outcome
    {
        public required bool IsSuccess { get; init; }
        public string? ExceptionMessage { get; init; }
    }

    public class Outcome<T> : Outcome
    {
        public T? Data { get; init; }
    }
}
