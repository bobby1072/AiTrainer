using AiTrainer.Web.Domain.Models.ApiModels.Request;
using BT.Common.FastArray.Proto;

namespace AiTrainer.Web.Domain.Models.Extensions
{
    public static class FileCollectionExtensions
    {
        public static FileCollection FromInput(
            FileCollectionSaveInput fileCollectionInput,
            Guid userId
        )
        {
            return new FileCollection
            {
                UserId = userId,
                CollectionName = fileCollectionInput.CollectionName,
                DateCreated = fileCollectionInput.DateCreated ?? DateTime.UtcNow,
                DateModified = fileCollectionInput.DateModified ?? DateTime.UtcNow,
                ParentId = fileCollectionInput.ParentId,
                Id = fileCollectionInput.Id,
                AutoFaissSync = fileCollectionInput.AutoFaissSync ?? false,
                CollectionDescription = fileCollectionInput.CollectionDescription,
            };
        }
    }
}
