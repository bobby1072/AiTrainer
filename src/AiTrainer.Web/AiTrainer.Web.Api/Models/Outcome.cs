namespace AiTrainer.Web.Api.Models
{
    internal class Outcome
    {
        public bool IsSuccess { get; init; }
        public string? ExceptionMessage { get; init; }
    }

    internal class Outcome<T> : Outcome
    {
        public T? Data { get; init; }
    }
}
