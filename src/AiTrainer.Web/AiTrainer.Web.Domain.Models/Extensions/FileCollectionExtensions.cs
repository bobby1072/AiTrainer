using AiTrainer.Web.Common.Models.ApiModels.Request;

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
                CollectionDescription = fileCollectionInput.CollectionDescription,
            };
        }
    }
}
