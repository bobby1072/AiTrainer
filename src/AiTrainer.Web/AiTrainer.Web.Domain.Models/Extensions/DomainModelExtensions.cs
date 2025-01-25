using System.Reflection;
using AiTrainer.Web.Domain.Models.Attributes;

namespace AiTrainer.Web.Domain.Models.Extensions
{
    public static class DomainModelExtensions
    {
        public static bool ValidateAgainstOriginal<TModel, TId>(
            this TModel originalModel,
            TModel checkAgainst
        )
            where TModel : DomainModel<TModel, TId>
        {
            var allPropertiesToCheck = checkAgainst.GetType().GetProperties();
            for (var i = 0; i < allPropertiesToCheck.Length; i++)
            {
                var property = allPropertiesToCheck[i];
                if (
                    property?.GetCustomAttribute<LockedDataAttribute>() is not null
                    && property.GetValue(originalModel)?.Equals(property.GetValue(checkAgainst))
                        is false
                )
                {
                    return false;
                }
            }
            return true;
        }

        public static void RemoveSensitive(this DomainModel<object, object> originalModel)
        {
            var allProperties = originalModel.GetType().GetProperties();
            for (var i = 0; i < allProperties.Length; i++)
            {
                var property = allProperties[i];
                var foundProp = property.GetValue(originalModel);
                if (foundProp is DomainModel<object, object> deepBaseModel)
                {
                    deepBaseModel.RemoveSensitive();
                }
                else if (foundProp is IEnumerable<DomainModel<object, object>> deepBaseModels)
                {
                    deepBaseModels.RemoveSensitive();
                }
                else if (property?.GetCustomAttribute<SensitivePropertyAttribute>() is not null)
                {
                    property.SetValue(originalModel, null);
                }
            }
        }

        public static void RemoveSensitive(
            this IEnumerable<DomainModel<object, object>> originalModels
        )
        {
            foreach (var model in originalModels)
            {
                model.RemoveSensitive();
            }
        }
    }
}
