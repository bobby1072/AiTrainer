using AiTrainer.Web.Domain.Services.Concrete;
using AiTrainer.Web.Domain.Services.Models;
using AiTrainer.Web.TestBase;
using BT.Common.OperationTimer.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AiTrainer.Web.Domain.Services.Tests;

public sealed class BatchedAsyncOperationExecutorTests: AiTrainerTestBase
{
    [Fact]
    public async Task Should_Process_Items_In_Batches()
    {
        // Arrange
        var processedBatches = new List<List<int>>();
        var handler = new Func<IReadOnlyCollection<int>, CancellationToken, Task>((items, _) =>
        {
            processedBatches.Add(new List<int>(items));
            return Task.CompletedTask;
        });

        var options = GetOptions(handler, batchSize: 3);
        var loggerMock = new Mock<ILogger<BatchedAsyncOperationExecutor<int>>>();
        var executor = new BatchedAsyncOperationExecutor<int>(loggerMock.Object, options);

        // Act
        await executor.DoWorkAsync(1, 2, 3, 4, 5, 6, 7);

        // Assert
        Assert.Equal(3, processedBatches.Count);
        Assert.Equal(new List<int> { 1, 2, 3 }, processedBatches[0]);
        Assert.Equal(new List<int> { 4, 5, 6 }, processedBatches[1]);
        Assert.Equal(new List<int> { 7 }, processedBatches[2]);
    }

    [Fact]
    public async Task Should_Handle_Exception_Without_Rethrow()
    {
        // Arrange
        var handler = new Func<IReadOnlyCollection<int>, CancellationToken, Task>((_, _) =>
        {
            throw new InvalidOperationException("fail");
        });

        var options = GetOptions(handler, rethrow: false);
        var loggerMock = new Mock<ILogger<BatchedAsyncOperationExecutor<int>>>();
        var executor = new BatchedAsyncOperationExecutor<int>(loggerMock.Object, options);

        // Act & Assert: Should NOT throw
        var ex = await Record.ExceptionAsync(() => executor.DoWorkAsync(1, 2));
        Assert.Null(ex);
    }

    [Fact]
    public async Task Should_Rethrow_On_Exception_When_Configured()
    {
        // Arrange
        var handler = new Func<IReadOnlyCollection<int>, CancellationToken, Task>((_, _) => throw new InvalidOperationException("fail"));

        var options = GetOptions(handler, rethrow: true);
        var loggerMock = new Mock<ILogger<BatchedAsyncOperationExecutor<int>>>();
        var executor = new BatchedAsyncOperationExecutor<int>(loggerMock.Object, options);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<OperationTimerException>(() => executor.DoWorkAsync(1, 2));
        
        Assert.IsType<InvalidOperationException>(ex.InnerException);
        Assert.Equal("fail", ex.InnerException?.Message);
    }

    [Fact]
    public async Task Should_Delay_Between_Batches()
    {
        // Arrange
        var timestamps = new List<DateTime>();
        var handler = new Func<IReadOnlyCollection<int>, CancellationToken, Task>((_, _) =>
        {
            timestamps.Add(DateTime.UtcNow);
            return Task.CompletedTask;
        });

        var options = GetOptions(handler, batchSize: 1, delayMs: 50); // delay between each batch
        var executor = new BatchedAsyncOperationExecutor<int>(new NullLogger<BatchedAsyncOperationExecutor<int>>(), options);

        // Act
        await executor.DoWorkAsync(1, 2);

        // Assert
        Assert.Equal(2, timestamps.Count);
        var delay = timestamps[1] - timestamps[0];
        Assert.True(delay >= TimeSpan.FromMilliseconds(40)); // slight tolerance
    }
    
    private static BatchedAsyncOperationExecutorOptions<int> GetOptions(
        Func<IReadOnlyCollection<int>, CancellationToken, Task> handler,
        int batchSize = 2,
        int delayMs = 10,
        bool rethrow = false)
    {
        return new BatchedAsyncOperationExecutorOptions<int>
        {
            BatchExecutionInterval = TimeSpan.FromMilliseconds(delayMs),
            BatchSize = batchSize,
            SingleBatchHandler = handler,
            ReThrowOnBatchException = rethrow,
            CorrelationId = Guid.NewGuid().ToString(),
            CancellationToken = CancellationToken.None
        };
    }
}
