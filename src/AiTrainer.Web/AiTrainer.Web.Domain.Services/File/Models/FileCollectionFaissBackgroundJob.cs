
namespace AiTrainer.Web.Domain.Services.File.Models;

public abstract class FileCollectionFaissBackgroundJob
{
    public Guid? CollectionId { get; init; }
    public required Domain.Models.User CurrentUser { get; init; }

    internal abstract Task ExecuteFaissJobAsync(IServiceProvider sp, CancellationToken ct = default);
}