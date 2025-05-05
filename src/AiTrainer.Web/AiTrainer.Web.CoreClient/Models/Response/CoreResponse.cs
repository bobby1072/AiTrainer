
namespace AiTrainer.Web.CoreClient.Models.Response
{
    internal record CoreResponse
    {
        public bool IsSuccess { get; init; }
        public string? ExceptionMessage { get; init; }
    }
    internal sealed record CoreResponse<T>: CoreResponse
    {
        public T? Data { get; init; }
    }
}
