using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Utils;
using BT.Common.FastArray.Proto;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AiTrainer.Web.Domain.Services.File.Concrete
{
    internal class FileCollectionProcessingManager : IFileCollectionProcessingManager
    {
        private readonly IFileCollectionRepository _repository;
        private readonly ILogger<FileCollectionProcessingManager> _logger;
        private readonly IValidator<FileCollection> _validator;
        private readonly IFileDocumentRepository _fileDocumentRepository;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public FileCollectionProcessingManager(
            IFileCollectionRepository repository,
            ILogger<FileCollectionProcessingManager> logger,
            IValidator<FileCollection> validator,
            IFileDocumentRepository fileDocumentRepository,
            IHttpContextAccessor? httpContextAccessor =  null
        )
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _validator = validator;
            _fileDocumentRepository = fileDocumentRepository;
        }

        public async Task<FileCollection> GetFileCollectionWithContents(Guid fileCollectionId, Domain.Models.User currentUser)
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(GetFileCollectionWithContents),
                correlationId
            );

            var foundCollection = await EntityFrameworkUtils.TryDbOperation(
                () => _repository.GetOne(fileCollectionId, nameof(FileCollectionEntity.Documents))
            );

            if (foundCollection?.IsSuccessful is false || foundCollection?.Data is null)
            {
                throw new ApiException("Could not find file collection with that id");
            }

            if (foundCollection.Data.UserId != currentUser.Id)
            {
                throw new ApiException(
                    "You do not have permission to access this file collection",
                    HttpStatusCode.Unauthorized
                );
            }

            if (foundCollection.Data.Documents is null || foundCollection.Data.Documents.Count < 1)
            {
                throw new ApiException(
                    "No documents within file collection",
                    HttpStatusCode.BadRequest
                );
            }

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(GetFileCollectionWithContents),
                correlationId
            );

            return foundCollection.Data;
        }

        public async Task<FileCollection> SaveFileCollection(
            FileCollectionSaveInput fileCollectionInput,
            Domain.Models.User currentUser
        )
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(SaveFileCollection),
                correlationId
            );

            var createdCollection = FileCollectionExtensions.FromInput(
                fileCollectionInput,
                (Guid)currentUser.Id!
            );

            var hasId = createdCollection.Id is not null;

            if (!hasId)
            {
                createdCollection.ApplyCreationDefaults();
            }

            var isValid = await _validator.ValidateAsync(createdCollection);

            if (!isValid.IsValid)
            {
                throw new ApiException("Collection is not valid", HttpStatusCode.BadRequest);
            }

            if (hasId)
            {
                _logger.LogInformation(
                    "Attempting to retrieve collection with id {CreatedCollectionId} for userId {UserId} and correlationId {CorrelationId}",
                    createdCollection.Id,
                    currentUser.Id,
                    correlationId
                );
                var foundOne = await _repository.GetOne((Guid)createdCollection.Id!);
                if (foundOne?.Data is null)
                {
                    throw new ApiException(
                        "Cannot find collection to update",
                        HttpStatusCode.BadRequest
                    );
                }

                if (
                    !createdCollection.ValidateAgainstOriginal<FileCollection, Guid?>(foundOne.Data)
                )
                {
                    throw new ApiException("Cannot edit those fields", HttpStatusCode.BadRequest);
                }
                createdCollection.DateCreated = foundOne.Data.DateCreated.ToUniversalTime();
                createdCollection.DateModified = DateTime.UtcNow.ToUniversalTime();
            }

            if (createdCollection.ParentId is Guid foundParentId)
            {
                var foundSingleParent = await EntityFrameworkUtils.TryDbOperation(
                    () => _repository.GetOne(foundParentId),
                    _logger
                );
                if (foundSingleParent?.Data?.UserId != currentUser.Id)
                {
                    throw new ApiException("Collection is not valid", HttpStatusCode.BadRequest);
                }
            }

            _logger.LogInformation(
                "{ActionName} attempting to {SaveMode} collection: {CreatedCollection}",
                nameof(SaveFileCollection),
                hasId ? "update" : "create",
                createdCollection
            );
            var newlySavedCollection = await EntityFrameworkUtils.TryDbOperation(
                () =>
                    hasId
                        ? _repository.Update([createdCollection])
                        : _repository.Create([createdCollection]),
                _logger
            );

            if (newlySavedCollection?.IsSuccessful != true)
            {
                throw new ApiException(
                    $"Failed to {(hasId ? "update" : "create")} file collection"
                );
            }
            _logger.LogInformation(
                "Exiting {Action} successfully for correlationId {CorrelationId}",
                nameof(SaveFileCollection),
                correlationId
            );
            return newlySavedCollection.Data.First();
        }

        public async Task<Guid> DeleteFileCollection(Guid collectionId, Domain.Models.User currentUser)
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(DeleteFileCollection),
                correlationId
            );

            var deletedId = await EntityFrameworkUtils.TryDbOperation(
                () => _repository.Delete(collectionId, (Guid)currentUser.Id!),
                _logger
            );

            if (deletedId?.IsSuccessful != true)
            {
                throw new ApiException(
                    "Could not delete document"
                );
            }
            _logger.LogInformation(
                "Exiting {Action} for correlationId {CorrelationId}",
                nameof(DeleteFileCollection),
                correlationId
            );
            return deletedId.Data.First();
        }

        public async Task<FlatFileDocumentPartialCollection> GetOneLayerFileDocPartialsAndCollections(
            Domain.Models.User currentUser,
            Guid? collectionId = null
        )
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(GetOneLayerFileDocPartialsAndCollections),
                correlationId
            );
            var collectionsJob = EntityFrameworkUtils.TryDbOperation(
                () =>
                    collectionId is null
                        ? _repository.GetTopLevelCollectionsForUser((Guid)currentUser.Id!)
                        : _repository.GetManyCollectionsForUserIncludingSelf(
                            (Guid)collectionId!,
                            (Guid)currentUser.Id!
                        ),
                _logger
            );
            var partialDocumentsJob = EntityFrameworkUtils.TryDbOperation(
                () =>
                    collectionId is null
                        ? _fileDocumentRepository.GetTopLevelDocumentPartialsForUser(
                            (Guid)currentUser.Id!,
                            nameof(FileDocumentEntity.MetaData)
                        )
                        : _fileDocumentRepository.GetManyDocumentPartialsByCollectionId(
                            (Guid)collectionId!,
                            (Guid)currentUser.Id!,
                            nameof(FileDocumentEntity.MetaData)
                        ),
                _logger
            );

            await Task.WhenAll(collectionsJob, partialDocumentsJob);
            var collections = (await collectionsJob)?.Data ?? [];
            var partialDocuments = (await partialDocumentsJob)?.Data ?? [];

            _logger.LogInformation(
                "Exiting {Action} for correlationId {CorrelationId}",
                nameof(GetOneLayerFileDocPartialsAndCollections),
                correlationId
            );

            return new FlatFileDocumentPartialCollection
            {
                Self = collections.FastArrayFirstOrDefault(x => x.Id == collectionId),
                FileCollections = collections.FastArrayWhere(x => x.Id != collectionId).ToArray(),
                FileDocuments = partialDocuments,
            };
        }
    }
}
