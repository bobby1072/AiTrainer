namespace AiTrainer.Web.Common.Models.DomainModels
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
