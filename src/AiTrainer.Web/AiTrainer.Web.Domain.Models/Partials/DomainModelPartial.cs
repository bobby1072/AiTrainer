namespace AiTrainer.Web.Domain.Models.Partials
{
    public abstract record DomainModelPartial<TEquatable, TId> : DomainModel<TEquatable, TId>
        where TEquatable : DomainModelPartial<TEquatable, TId> { }
}
