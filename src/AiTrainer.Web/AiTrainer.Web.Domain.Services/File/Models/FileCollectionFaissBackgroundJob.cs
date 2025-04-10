using AiTrainer.Web.Domain.Services.File.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Models;

public abstract record FileCollectionFaissBackgroundJob
{
    public Guid? CollectionId { get; init; }
    public required Domain.Models.User CurrentUser { get; init; }
}
public abstract record FileCollectionFaissBackgroundJob<TProcessingManager>: FileCollectionFaissBackgroundJob where TProcessingManager: IFileCollectionFaissProcessingManager
{
    internal abstract Task JobProcessToRun(TProcessingManager manager, CancellationToken ct);
}