using System.Threading.Channels;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.File.Models;

namespace AiTrainer.Web.Domain.Services.File.Concrete;

internal class FileCollectionFaissSyncBackgroundJobQueue : IFileCollectionFaissSyncBackgroundJobQueue
{
    private readonly Channel<FileCollectionFaissSyncBackgroundJob> _channel = Channel.CreateUnbounded<FileCollectionFaissSyncBackgroundJob>();

    public async Task Enqueue(FileCollectionFaissSyncBackgroundJob job) => await _channel.Writer.WriteAsync(job).AsTask();

    public async Task<FileCollectionFaissSyncBackgroundJob> DequeueAsync(CancellationToken cancellationToken = default)
        => await _channel.Reader.ReadAsync(cancellationToken);

    public void Dispose() => _channel.Writer.Complete();
}