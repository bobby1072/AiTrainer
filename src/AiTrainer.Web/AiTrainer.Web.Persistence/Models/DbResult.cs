namespace AiTrainer.Web.Persistence.Models
{
    public record DbResult
    {
        public bool IsSuccessful { get; init; }

        public DbResult(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }

    public record DbResult<T> : DbResult
    {
        public T Data { get; init; }

        public DbResult(bool isSuccess, T data)
            : base(isSuccess)
        {
            Data = data;
        }
    }
}
