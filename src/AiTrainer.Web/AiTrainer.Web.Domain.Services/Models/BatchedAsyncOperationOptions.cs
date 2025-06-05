namespace AiTrainer.Web.Domain.Services.Models;

internal sealed class BatchedAsyncOperationOptions<TInputItem>
{
    public required TimeSpan BatchExecutionInterval { get; init; }
    public required int BatchSize { get; init; }
    public required Func<IReadOnlyCollection<TInputItem>, CancellationToken, Task> SingleBatchHandler { get; init; }
    public bool ReThrowOnBatchException { get; init; }
    public string? CorrelationId { get; init; }
    public CancellationToken CancellationToken { get; init; } = default;
}