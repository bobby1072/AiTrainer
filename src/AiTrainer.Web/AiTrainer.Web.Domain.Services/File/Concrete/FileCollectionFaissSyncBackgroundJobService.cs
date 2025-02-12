using System.Collections.Concurrent;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.File.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.File.Concrete;

internal class FileCollectionFaissSyncBackgroundJobService: BackgroundService, IFileCollectionFaissSyncBackgroundJobService
{
    private readonly ILogger<FileCollectionFaissSyncBackgroundJobService> _logger;
    private readonly IFileCollectionFaissSyncProcessingManager  _syncProcessingManager;
    private readonly BlockingCollection<FileCollectionFaissSyncBackgroundJob> _jobQueue = new();

    public FileCollectionFaissSyncBackgroundJobService(ILogger<FileCollectionFaissSyncBackgroundJobService> logger,
        IFileCollectionFaissSyncProcessingManager  syncProcessingManager)
    {
        _logger = logger;
        _syncProcessingManager = syncProcessingManager;
    }

    public void EnqueueJob(FileCollectionFaissSyncBackgroundJob job)
    {
        _jobQueue.Add(job);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{BackgroundServiceName} is starting", nameof(FileCollectionFaissSyncBackgroundJobService));

        foreach (var job in _jobQueue.GetConsumingEnumerable(stoppingToken))
        {
            if (stoppingToken.IsCancellationRequested)
                break;

            try
            {
                _logger.LogInformation("--------Processing faiss sync request in {BackgroundServiceName} for collectionId {CollectionId} and userId {UserId}",
                    nameof(FileCollectionFaissSyncBackgroundJobService),
                    job.CollectionId,
                    job.User.Id);
                await job.SyncProcess.Compile().Invoke(_syncProcessingManager, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exceptions occurred during execution faiss sync job for collectionId {CollectionId} and userId {UserId}",
                    job.CollectionId,
                    job.User.Id);
            }
        }
    }

    public override void Dispose()
    {
        _jobQueue.CompleteAdding();
        base.Dispose();
    }
}