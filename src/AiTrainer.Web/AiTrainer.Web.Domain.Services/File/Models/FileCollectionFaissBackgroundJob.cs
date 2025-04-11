
namespace AiTrainer.Web.Domain.Services.File.Models;

internal abstract class FileCollectionFaissBackgroundJob
{
    public Guid? CollectionId { get; init; }
    public required Domain.Models.User CurrentUser { get; init; }

    public abstract Task ExecuteFaissJob(IServiceProvider sp, CancellationToken ct = default);
}