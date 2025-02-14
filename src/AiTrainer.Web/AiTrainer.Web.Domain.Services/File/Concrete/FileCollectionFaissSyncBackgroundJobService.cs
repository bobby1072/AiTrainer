using AiTrainer.Web.Domain.Services.Concrete;
using AiTrainer.Web.Domain.Services.File.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.File.Concrete;

internal class FileCollectionFaissSyncBackgroundJobService: BackgroundService
{
    private readonly ILogger<FileCollectionFaissSyncBackgroundJobService> _logger;
    private readonly IFileCollectionFaissSyncBackgroundJobQueue _jobQueue;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public FileCollectionFaissSyncBackgroundJobService(ILogger<FileCollectionFaissSyncBackgroundJobService> logger,
        IFileCollectionFaissSyncBackgroundJobQueue jobQueue,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _jobQueue = jobQueue;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("{BackgroundServiceName} is starting", nameof(FileCollectionFaissSyncBackgroundJobService));

        while (!stoppingToken.IsCancellationRequested)
        {
            var job = await _jobQueue.DequeueAsync(stoppingToken);
            try
            {
                _logger.LogDebug("--------Processing faiss sync request in {BackgroundServiceName} for collectionId {CollectionId} and userId {UserId}--------",
                    nameof(FileCollectionFaissSyncBackgroundJobService),
                    job.CollectionId,
                    job.User.Id);
                await using var scope = _serviceScopeFactory.CreateAsyncScope();
                var syncManager = scope.ServiceProvider.GetRequiredService<IFileCollectionFaissSyncProcessingManager>();
                
                await job.SyncProcess.Compile().Invoke(syncManager, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exceptions occurred during execution of background faiss sync job for collectionId {CollectionId} and userId {UserId}",
                    job.CollectionId,
                    job.User.Id);
            }
            _logger.LogDebug("--------Processing finished for faiss sync request for collectionId {CollectionId} and userId {UserId}--------",
                job.CollectionId,
                job.User.Id);    
                
        }
    }

    public override void Dispose()
    {
        _jobQueue.Dispose();
        base.Dispose();
    }
}