namespace AiTrainer.Web.Domain.Models.Attributes
{
    /// <summary>
    /// This attribute is used to mark a property or model type as locked.
    /// Meaning that the property or model type should not be updated and persisted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LockedDataAttribute : Attribute { }
}
