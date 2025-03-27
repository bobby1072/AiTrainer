using AiTrainer.Web.Domain.Models.Attributes;
using System.Reflection;

namespace AiTrainer.Web.Domain.Models.Extensions
{
    public static class DomainModelExtensions
    {
        public static bool ValidateAgainstOriginal(
            this DomainModel originalModel,
            DomainModel checkAgainst
        )
        {
            var entType = checkAgainst.GetType();
            if (
                entType.GetCustomAttribute<LockedDataAttribute>() is not null
                && originalModel.Equals(checkAgainst) is false
            )
            {
                return false;
            }
            var allPropertiesToCheck = entType.GetProperties();
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

        public static void RemoveSensitive(this DomainModel originalModel)
        {
            var allProperties = originalModel.GetType().GetProperties();
            for (var i = 0; i < allProperties.Length; i++)
            {
                var property = allProperties[i];
                var foundProp = property.GetValue(originalModel);
                if (foundProp is DomainModel deepBaseModel)
                {
                    deepBaseModel.RemoveSensitive();
                }
                else if (foundProp is IEnumerable<DomainModel> deepBaseModels)
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
            this IEnumerable<DomainModel> originalModels
        )
        {
            foreach (var model in originalModels)
            {
                model.RemoveSensitive();
            }
        }
    }
}
