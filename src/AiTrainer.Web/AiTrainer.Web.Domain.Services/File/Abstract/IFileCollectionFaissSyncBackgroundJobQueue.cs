using AiTrainer.Web.Domain.Services.File.Models;

namespace AiTrainer.Web.Domain.Services.File.Abstract;

internal interface IFileCollectionFaissSyncBackgroundJobQueue: IDisposable
{
    Task EnqueueAsync(FileCollectionFaissBackgroundJob job, CancellationToken cancellationToken = default);
    internal Task<FileCollectionFaissBackgroundJob> DequeueAsync(CancellationToken cancellationToken = default);
}