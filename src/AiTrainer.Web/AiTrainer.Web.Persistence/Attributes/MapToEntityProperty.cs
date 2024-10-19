namespace AiTrainer.Web.Persistence.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapToEntityAttribute : Attribute
    {
        public string PropertyName { get; }

        public MapToEntityAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
