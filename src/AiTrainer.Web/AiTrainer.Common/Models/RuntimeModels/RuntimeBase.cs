namespace AiTrainer.Web.Common.Models.RuntimeModels
{
    public abstract class RuntimeBase
    {
        public abstract bool Equals(RuntimeBase? other);

        public new virtual int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
