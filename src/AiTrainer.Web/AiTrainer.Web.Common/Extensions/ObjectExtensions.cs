namespace AiTrainer.Web.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static T? GetPropertyValue<T>(this object? value, string propertyName)
        {
            if (value is null)
            {
                return default;
            }

            var valType = value.GetType();
            var correctProperties = valType
                .GetProperties()
                .SingleOrDefault(x => x.Name == propertyName && x.PropertyType == typeof(T));

            return correctProperties?.GetValue(value) is T result ? result : default;
        }
    }
}
