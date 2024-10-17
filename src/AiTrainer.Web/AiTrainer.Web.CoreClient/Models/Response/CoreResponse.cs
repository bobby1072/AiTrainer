namespace AiTrainer.Web.CoreClient.Models.Response
{
    internal record CoreResponse<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public string? ExceptionMessage { get; init; }
    }
}
