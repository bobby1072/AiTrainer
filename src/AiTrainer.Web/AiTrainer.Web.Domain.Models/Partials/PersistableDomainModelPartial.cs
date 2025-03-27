namespace AiTrainer.Web.Domain.Models.Partials
{
    public abstract class PersistableDomainModelPartial<TEquatable, TId> : PersistableDomainModel<TEquatable, TId> where TEquatable : DomainModel<TEquatable>
    { }
}