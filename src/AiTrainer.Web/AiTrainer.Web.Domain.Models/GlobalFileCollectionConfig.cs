namespace AiTrainer.Web.Domain.Models;

public class GlobalFileCollectionConfig : PersistableDomainModel<GlobalFileCollectionConfig, long?>
{
    public bool AutoFaissSync { get; init; }
    public required Guid UserId { get; init; }
    public override bool Equals(GlobalFileCollectionConfig? obj)
    {
        return obj?.AutoFaissSync == AutoFaissSync &&
                obj.UserId == UserId;
    }
}