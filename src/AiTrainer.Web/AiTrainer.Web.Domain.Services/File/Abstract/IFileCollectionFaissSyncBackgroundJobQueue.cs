using AiTrainer.Web.Domain.Services.File.Models;

namespace AiTrainer.Web.Domain.Services.File.Abstract;

internal interface IFileCollectionFaissSyncBackgroundJobQueue
{
    Task Enqueue(FileCollectionFaissSyncBackgroundJob job);
    Task<FileCollectionFaissSyncBackgroundJob> DequeueAsync(CancellationToken cancellationToken = default);
    void Dispose();
}