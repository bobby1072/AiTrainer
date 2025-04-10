using AiTrainer.Web.Domain.Services.File.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Models;

internal abstract class FileCollectionFaissBackgroundJob
{
    public Guid? CollectionId { get; init; }
    public required Domain.Models.User CurrentUser { get; init; }
}
internal abstract class FileCollectionFaissBackgroundJob<TProcessingManager>: FileCollectionFaissBackgroundJob where TProcessingManager: IFileCollectionFaissProcessingManager
{
    public abstract Task JobProcessToRun(TProcessingManager manager, CancellationToken ct);
}