﻿using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Entities;

namespace AiTrainer.Web.Persistence.Extensions
{
    internal static class EntityFrameworkExtensions
    {
        public static GlobalFileCollectionConfigEntity ToEntity(
            this GlobalFileCollectionConfig globalFileCollectionConfig
        )
        {
            var entity = new GlobalFileCollectionConfigEntity
            {
                UserId = globalFileCollectionConfig.UserId,
                AutoFaissSync = globalFileCollectionConfig.AutoFaissSync,
            };

            if (globalFileCollectionConfig.Id is long foundId)
            {
                entity.Id = foundId;
            }

            return entity;
        }

        public static FileDocumentMetaDataEntity ToEntity(this FileDocumentMetaData formInput)
        {
            var entity = new FileDocumentMetaDataEntity
            {
                DocumentId = formInput.DocumentId,
                Title = formInput.Title,
                Author = formInput.Author,
                Keywords = formInput.Keywords,
                Creator = formInput.Creator,
                CreationDate = formInput.CreationDate,
                ModifiedDate = formInput.ModifiedDate,
                NumberOfPages = formInput.NumberOfPages,
                IsEncrypted = formInput.IsEncrypted,
                Producer = formInput.Producer,
                Subject = formInput.Subject,
                ExtraData = formInput.ExtraData,
            };

            if (formInput.Id is long foundId)
            {
                entity.Id = foundId;
            }

            return entity;
        }

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
                FaissSynced = fileDocument.FaissSynced,
                DateCreated = fileDocument.DateCreated,
                FileType = (int)fileDocument.FileType,
                FileData = fileDocument.FileData,
                FileName = fileDocument.FileName,
                FileDescription = fileDocument.FileDescription,
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
                CollectionDescription = fileCollection.CollectionDescription,
                AutoFaissSync = fileCollection.AutoFaissSync,
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
                UserId = fileCollectionFaiss.UserId,
                FaissJson = fileCollectionFaiss.FaissJson,
                DateCreated = fileCollectionFaiss.DateCreated,
                DateModified = fileCollectionFaiss.DateModified,
            };
            if (fileCollectionFaiss.Id is not null)
            {
                entity.Id = (long)fileCollectionFaiss.Id!;
            }

            return entity;
        }

        public static SharedFileCollectionMemberEntity ToEntity(
            this SharedFileCollectionMember fileCollectionMember
        )
        {
            var entity = new SharedFileCollectionMemberEntity
            {
                UserId = fileCollectionMember.UserId,
                CollectionId = fileCollectionMember.CollectionId,
                ParentSharedMemberId = fileCollectionMember.ParentSharedMemberId,
                CanCreateDocuments = fileCollectionMember.CanCreateDocuments,
                CanDownloadDocuments = fileCollectionMember.CanDownloadDocuments,
                CanRemoveDocuments = fileCollectionMember.CanRemoveDocuments,
                CanViewDocuments = fileCollectionMember.CanViewDocuments,
                CanSimilaritySearch = fileCollectionMember.CanSimilaritySearch,
            };

            if (fileCollectionMember.Id is not null)
            {
                entity.Id = (Guid)fileCollectionMember.Id!;
            }

            return entity;
        }
    }
}
