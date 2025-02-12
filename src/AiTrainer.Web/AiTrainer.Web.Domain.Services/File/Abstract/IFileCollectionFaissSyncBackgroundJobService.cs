using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.File.Models;
using Microsoft.Extensions.Hosting;

namespace AiTrainer.Web.Domain.Services.File.Abstract;

internal interface IFileCollectionFaissSyncBackgroundJobService: IHostedService, IDisposable, IDomainService
{
    void EnqueueJob(FileCollectionFaissSyncBackgroundJob job);
}