namespace AiTrainer.Web.Domain.Models
{
    public abstract class DomainModel
    {
        public abstract bool Equals(DomainModel? other);

        public new virtual int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
