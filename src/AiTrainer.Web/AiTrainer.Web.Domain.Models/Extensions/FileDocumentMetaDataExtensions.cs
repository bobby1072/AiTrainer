using System.Reflection;

namespace AiTrainer.Web.Domain.Models.Extensions
{
    public static class FileDocumentMetaDataExtensions
    {
        public static readonly PropertyInfo[] _fileDocumentMetaDataPropertiesToFillIntoDict =
         typeof(FileDocumentMetaData)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => !x.PropertyType.IsAssignableTo(typeof(Dictionary<string, string?>)))
            .ToArray();
        public static Dictionary<string, string> ToDictionary(this FileDocumentMetaData? metadata)
        {
            if (metadata is null) return [];
            var originalDictionary = metadata?.
                ExtraData?
                    .Where(x => x.Value is not null)
                    .Select(x => new KeyValuePair<string, string>(x.Key, x.Value!))
                    .ToDictionary()
             ?? [];

            foreach (var property in _fileDocumentMetaDataPropertiesToFillIntoDict)
            {
                var foundVal = property.GetValue(metadata)?.ToString();
                if (foundVal is null) continue;
                originalDictionary.Add(property.Name, foundVal);
            }

            return originalDictionary;
        }
    }
}