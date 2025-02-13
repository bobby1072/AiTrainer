using AiTrainer.Web.Domain.Services.File.Models;

namespace AiTrainer.Web.Domain.Services.File.Abstract;

public interface IFileCollectionFaissSyncBackgroundJobQueue: IDisposable
{
    Task Enqueue(FileCollectionFaissSyncBackgroundJob job);
    Task<FileCollectionFaissSyncBackgroundJob> DequeueAsync(CancellationToken cancellationToken = default);
}