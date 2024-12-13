namespace AiTrainer.Web.Common.Models.ApiModels.Response
{
    public record Outcome
    {
        public virtual bool IsSuccess => string.IsNullOrEmpty(ExceptionMessage);
        public string? ExceptionMessage { get; init; }
    }

    public record Outcome<T> : Outcome
    {
        public T? Data { get; init; }
    }
}
