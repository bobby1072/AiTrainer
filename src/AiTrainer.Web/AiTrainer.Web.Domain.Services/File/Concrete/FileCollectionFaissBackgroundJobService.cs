using AiTrainer.Web.Domain.Services.File.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.File.Concrete;

internal class FileCollectionFaissBackgroundJobService : BackgroundService
{
    private readonly ILogger<FileCollectionFaissBackgroundJobService> _logger;
    private readonly IFileCollectionFaissSyncBackgroundJobQueue _jobQueue;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public FileCollectionFaissBackgroundJobService(
        ILogger<FileCollectionFaissBackgroundJobService> logger,
        IFileCollectionFaissSyncBackgroundJobQueue jobQueue,
        IServiceScopeFactory serviceScopeFactory
    )
    {
        _logger = logger;
        _jobQueue = jobQueue;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override void Dispose()
    {
        _jobQueue.Dispose();
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "{BackgroundServiceName} is starting...",
            nameof(FileCollectionFaissBackgroundJobService)
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            var job = await _jobQueue.DequeueAsync(stoppingToken);
            var jobName = job.GetType().Name;
            _logger.LogDebug(
                "--------Processing faiss background job {JobName} in {BackgroundServiceName} for collectionId {CollectionId} and userId {UserId}--------",
                jobName,
                nameof(FileCollectionFaissBackgroundJobService),
                job.CollectionId,
                job.CurrentUser.Id
            );
            try
            {
                await using var scope = _serviceScopeFactory.CreateAsyncScope();

                await job.ExecuteFaissJobAsync(scope.ServiceProvider, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Exceptions occurred during execution of background faiss job for collectionId {CollectionId} and userId {UserId}",
                    job.CollectionId,
                    job.CurrentUser.Id
                );
            }
            _logger.LogDebug(
                "--------Processing finished for faiss background job {JobName} for collectionId {CollectionId} and userId {UserId}--------",
                jobName,
                job.CollectionId,
                job.CurrentUser.Id
            );
        }
    }
}
