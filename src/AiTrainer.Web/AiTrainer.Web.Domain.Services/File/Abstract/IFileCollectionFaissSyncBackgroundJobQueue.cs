using AiTrainer.Web.Domain.Services.File.Models;

namespace AiTrainer.Web.Domain.Services.File.Abstract;

public interface IFileCollectionFaissSyncBackgroundJobQueue: IDisposable
{
    Task Enqueue(FileCollectionFaissBackgroundJob job);
    Task<FileCollectionFaissBackgroundJob> DequeueAsync(CancellationToken cancellationToken = default);
}