using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Entities;

namespace AiTrainer.Web.Persistence.Extensions
{
    internal static class EntityFrameworkExtensions
    {
        public static UserEntity ToEntity(this User user)
        {
            var entity = new UserEntity
            {
                DateCreated = user.DateCreated,
                DateModified = user.DateModified,
                Email = user.Email,
                Name = user.Name,
            };
            if (user.Id is not null)
            {
                entity.Id = (Guid)user.Id!;
            }

            return entity;
        }

        public static FileDocumentEntity ToEntity(this FileDocument fileDocument)
        {
            var entity = new FileDocumentEntity
            {
                CollectionId = fileDocument.CollectionId,
                DateCreated = fileDocument.DateCreated,
                FileType = (int)fileDocument.FileType,
                FileData = fileDocument.FileData,
                FileName = fileDocument.FileName,
                FaissIndex = 
            };
            if (fileDocument.Id is not null)
            {
                entity.Id = (Guid)fileDocument.Id!;
            }

            return entity;
        }

        public static FileCollectionEntity ToEntity(this FileCollection fileCollection)
        {
            var entity = new FileCollectionEntity
            {
                DateCreated = fileCollection.DateCreated,
                DateModified = fileCollection.DateModified,
                CollectionName = fileCollection.CollectionName,
                UserId = fileCollection.UserId,
                ParentId = fileCollection.ParentId,
                FaissIndex = fileCollection.FaissIndex,
                FaissJson = fileCollection.FaissJson,
            };
            if (fileCollection.Id is not null)
            {
                entity.Id = (Guid)fileCollection.Id!;
            }

            return entity;
        }
    }
}
