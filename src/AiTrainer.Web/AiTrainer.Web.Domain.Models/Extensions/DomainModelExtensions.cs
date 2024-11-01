using AiTrainer.Web.Domain.Models.Attributes;
using System.Reflection;

namespace AiTrainer.Web.Domain.Models.Extensions
{
    public static class DomainModelExtensions
    {
        public static bool ValidateAgainstOriginal<TModel, TModelId>(this TModel originalModel,TModel checkAgainst) where TModel : DomainModel<TModelId>
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
        public static void RemoveSensitive<TModel, TModelId>(this TModel originalModel) where TModel : DomainModel<TModelId>
        {
            var allProperties = originalModel.GetType().GetProperties();
            for (var i = 0; i < allProperties.Length; i++)
            {
                var property = allProperties[i];
                var foundProp = property.GetValue(originalModel);
                if (foundProp is DomainModel<TModelId> deepBaseModel)
                {
                    deepBaseModel.RemoveSensitive<DomainModel<TModelId>, TModelId>();
                }
                else if (foundProp is IEnumerable<DomainModel<TModelId>> deepBaseModels)
                {
                    deepBaseModels.RemoveSensitive<DomainModel<TModelId>, TModelId>();
                }
                else if (property?.GetCustomAttribute<SensitivePropertyAttribute>() is not null)
                {
                    property.SetValue(originalModel, null);
                }
            }
        }
        public static void RemoveSensitive<TModel, TModelId>(this IEnumerable<TModel> originalModels) where TModel : DomainModel<TModelId>
        {
            foreach(var model in originalModels)
            {
                model.RemoveSensitive<TModel, TModelId>();
            }
        }
    }
}
