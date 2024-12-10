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
                UserId = fileDocument.UserId,
                DateCreated = fileDocument.DateCreated,
                FileType = (int)fileDocument.FileType,
                FileData = fileDocument.FileData,
                FileName = fileDocument.FileName,
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
            };
            if (fileCollection.Id is not null)
            {
                entity.Id = (Guid)fileCollection.Id!;
            }

            return entity;
        }

        public static FileCollectionFaissEntity ToEntity(
            this FileCollectionFaiss fileCollectionFaiss
        )
        {
            var entity = new FileCollectionFaissEntity
            {
                CollectionId = fileCollectionFaiss.CollectionId,
                FaissIndex = fileCollectionFaiss.FaissIndex,
                FaissJson = fileCollectionFaiss.FaissJson,
            };
            if (fileCollectionFaiss.Id is not null)
            {
                entity.Id = (long)fileCollectionFaiss.Id!;
            }

            return entity;
        }

        public static SolicitedDeviceTokenEntity ToEntity(this SolicitedDeviceToken deviceToken)
        {
            var entity = new SolicitedDeviceTokenEntity
            {
                InUse = deviceToken.InUse,
                SolicitedAt = deviceToken.SolicitedAt,
                ExpiresAt = deviceToken.ExpiresAt,
            };

            if (deviceToken.Id is not null)
            {
                entity.Id = (Guid)deviceToken.Id!;
            }

            return entity;
        }
    }
}
