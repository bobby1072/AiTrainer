using System.Threading.Channels;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.File.Models;

namespace AiTrainer.Web.Domain.Services.File.Concrete;

internal class FileCollectionFaissBackgroundJobQueue : IFileCollectionFaissSyncBackgroundJobQueue
{
    private readonly Channel<FileCollectionFaissBackgroundJob> _channel = Channel.CreateUnbounded<FileCollectionFaissBackgroundJob>();
    public Task EnqueueAsync(FileCollectionFaissBackgroundJob job, CancellationToken cancellationToken = default)
    {
        return _channel.Writer.WriteAsync(job, cancellationToken).AsTask();
    }

    public async Task<FileCollectionFaissBackgroundJob> DequeueAsync(CancellationToken cancellationToken = default)
        => await _channel.Reader.ReadAsync(cancellationToken);

    public void Dispose() => _channel.Writer.Complete();
}