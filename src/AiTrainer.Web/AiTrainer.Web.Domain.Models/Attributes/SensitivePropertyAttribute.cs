namespace AiTrainer.Web.Domain.Models.Attributes
{

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SensitivePropertyAttribute : Attribute
    {
    }
}
