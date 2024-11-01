namespace AiTrainer.Web.Persistence.Models
{
    public record DbResult
    {
        public bool IsSuccessful { get; init; }
    }
    public record DbResult<T>: DbResult
    {
        public T? Data { get; init; }    }
}
