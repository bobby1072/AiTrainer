namespace AiTrainer.Web.Domain.Models;

public class GlobalCollectionConfig : PersistableDomainModel<GlobalCollectionConfig, long>
{
    public bool AutoFaissSync { get; init; }

    public override bool Equals(GlobalCollectionConfig? obj)
    {
        return obj?.AutoFaissSync == AutoFaissSync;
    }
}