using AiTrainer.Web.Domain.Models.Attributes;
using System.Reflection;

namespace AiTrainer.Web.Domain.Models.Extensions
{
    public static class DomainModelExtensions
    {
        public static bool ValidateAgainstOriginal<TModel>(this TModel originalModel,TModel checkAgainst) where TModel : DomainModel<object>
        {
            var allPropertiesToCheck = checkAgainst.GetType().GetProperties();
            for (var i = 0; i < allPropertiesToCheck.Length; i++)
            {
                var property = allPropertiesToCheck[i];
                if (property?.GetCustomAttribute<LockedPropertyAttribute>() is not null && property.GetValue(originalModel)?.Equals(property.GetValue(checkAgainst)) is false)
                {
                    return false;
                }
            }
            return true;
        }
        public static void RemoveSensitive<TModel>(this TModel originalModel) where TModel : DomainModel<object>
        {
            var allProperties = originalModel.GetType().GetProperties();
            for (var i = 0; i < allProperties.Length; i++)
            {
                var property = allProperties[i];
                var foundProp = property.GetValue(originalModel);
                if (foundProp is DomainModel<object> deepBaseModel)
                {
                    deepBaseModel.RemoveSensitive();
                }
                else if (foundProp is IEnumerable<DomainModel<object>> deepBaseModels)
                {
                    deepBaseModels.RemoveSensitive();
                }
                else if (property?.GetCustomAttribute<SensitivePropertyAttribute>() is not null)
                {
                    property.SetValue(originalModel, null);
                }
            }
        }
        public static void RemoveSensitive<TModel>(this IEnumerable<TModel> originalModels) where TModel : DomainModel<object>
        {
            foreach(var model in originalModels)
            {
                model.RemoveSensitive();
            }
        }
    }
}
