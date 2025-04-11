namespace AiTrainer.Web.Domain.Models;

public abstract class DomainModel { }
public abstract class DomainModel<TEquatable>:  DomainModel, IEquatable<TEquatable> where TEquatable : DomainModel<TEquatable>
{
    public abstract bool Equals(TEquatable? obj);

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }    
}