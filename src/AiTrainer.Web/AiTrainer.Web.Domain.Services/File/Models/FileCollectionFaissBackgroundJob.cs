
namespace AiTrainer.Web.Domain.Services.File.Models;

internal abstract class FileCollectionFaissBackgroundJob<TSelf>: IEquatable<TSelf> where TSelf : FileCollectionFaissBackgroundJob<TSelf>
{
    public Guid? CollectionId { get; init; }
    public required Domain.Models.User CurrentUser { get; init; }

    public abstract Task ExecuteFaissJobAsync(IServiceProvider sp, CancellationToken ct = default);

    public virtual bool Equals(TSelf? other)
    {
        return CollectionId.Equals(other?.CollectionId) && CurrentUser.Equals(other?.CurrentUser);
    }
}