using AiTrainer.Web.Domain.Services.Models;
using BT.Common.OperationTimer.Proto;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.Concrete;

internal sealed class BatchedAsyncOperationExecutor<TInputItem>
{
    private readonly ILogger<BatchedAsyncOperationExecutor<TInputItem>> _logger;
    private readonly BatchedAsyncOperationExecutorOptions<TInputItem> _options;
    private readonly Queue<TInputItem> _queue = new();
    
    private int _batchSize => _options.BatchSize > 0 ? _options.BatchSize : 1;
    public BatchedAsyncOperationExecutor(
        ILogger<BatchedAsyncOperationExecutor<TInputItem>> logger,
        BatchedAsyncOperationExecutorOptions<TInputItem> options
    )
    {
        _logger = logger;
        _options = options;
    }
    public async Task DoWorkAsync(params TInputItem[] items)
    {
        foreach (var item in items)
        {
            _queue.Enqueue(item);
        }

        await TriggerBatchedAsyncOperationsAsync();
    }

    private async Task TriggerBatchedAsyncOperationsAsync()
    {
        _logger.LogDebug("Attempting to execute {NumberOfOperations} in batches of {BatchSize} for correlationId: {CorrelationId}",
            _queue.Count, 
            _batchSize,
            _options.CorrelationId
        );
        var batchesExecuted = 0;

        while (_queue.Count > 0)
        {
            var singleBatch = new List<TInputItem>();
            
            while (singleBatch.Count < _batchSize && _queue.TryDequeue(out var item))
            {
                singleBatch.Add(item);
            }
            if (singleBatch.Count == 0)
            {
                return;
            }
            var timeTaken = await OperationTimerUtils.TimeAsync(() => ExecuteBatch(singleBatch));
            
            _logger.LogDebug("Single batch of {NumberOfOperations} operations took {TimeTaken}ms to execute for correlationId: {CorrelationId}",
                singleBatch.Count, 
                timeTaken,
                _options.CorrelationId
            );
            if(batchesExecuted > 0 && _options.BatchExecutionInterval > TimeSpan.Zero)
            {
                await Task.Delay(_options.BatchExecutionInterval);
            }

            batchesExecuted++;
        }
    }

    private async Task ExecuteBatch(IReadOnlyCollection<TInputItem> items)
    {
        try
        {
            await _options.SingleBatchHandler.Invoke(items, _options.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Exception occurred while executing batched operations for correlationId: {CorrelationId}",
                _options.CorrelationId
            );

            if (_options.ReThrowOnBatchException)
            {
                throw;
            }
        }
    }
}