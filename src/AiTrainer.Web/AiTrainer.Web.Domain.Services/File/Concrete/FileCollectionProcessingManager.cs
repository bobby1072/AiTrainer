using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Utils;
using BT.Common.FastArray.Proto;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using AiTrainer.Web.Domain.Models.Views;

namespace AiTrainer.Web.Domain.Services.File.Concrete
{
    internal class FileCollectionProcessingManager : IFileCollectionProcessingManager
    {
        private readonly IFileCollectionRepository _repository;
        private readonly ILogger<FileCollectionProcessingManager> _logger;
        private readonly IValidator<FileCollection> _validator;
        private readonly IFileDocumentRepository _fileDocumentRepository;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly IRepository<SharedFileCollectionMemberEntity, Guid, SharedFileCollectionMember> _sharedFileCollectionMemberRepository;
        private readonly IValidator<IEnumerable<SharedFileCollectionMember>> _sharedFileCollectionMemberValidator;
        public FileCollectionProcessingManager(
            IFileCollectionRepository repository,
            ILogger<FileCollectionProcessingManager> logger,
            IValidator<FileCollection> validator,
            IFileDocumentRepository fileDocumentRepository,
            IRepository<SharedFileCollectionMemberEntity, Guid, SharedFileCollectionMember> sharedFileCollectionMemberRepository,
            IValidator<IEnumerable<SharedFileCollectionMember>> sharedFileCollectionMemberValidator,
            IHttpContextAccessor? httpContextAccessor =  null
        )
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _validator = validator;
            _fileDocumentRepository = fileDocumentRepository;
            _sharedFileCollectionMemberRepository = sharedFileCollectionMemberRepository;
            _sharedFileCollectionMemberValidator = sharedFileCollectionMemberValidator;
            _fileDocumentRepository = fileDocumentRepository;
        }

        public async Task<IReadOnlyCollection<SharedFileCollectionMember>> ShareFileCollectionAsync(SharedFileCollectionMemberSaveInput sharedFileColInput, Domain.Models.User currentUser)
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(ShareFileCollectionAsync),
                correlationId
            );
            
            var foundCollection = await EntityFrameworkUtils.TryDbOperation(
                () => _repository.GetOne(sharedFileColInput.CollectionId, nameof(FileCollectionEntity.SharedFileMembers)), _logger
            );

            if (foundCollection?.IsSuccessful is false || foundCollection?.Data is null)
            {
                throw new ApiException("Could not find file collection with that id");
            }

            if (foundCollection.Data.UserId != currentUser.Id)
            {
                throw new ApiException(
                    "You do not have permission to share this file collection",
                    HttpStatusCode.Unauthorized
                );
            }
            
            var sharedMembersToSave = CreateSharedFileCollectionMembers(sharedFileColInput, foundCollection.Data.SharedFileMembers ?? [], (Guid)currentUser.Id!);

            if (sharedMembersToSave.Length == 0)
            {
                return [];
            }

            if (sharedMembersToSave.Length + (foundCollection.Data.SharedFileMembers?.Count ?? 0) > 30)
            {
                throw new ApiException("you cannot share more than 30 users on a file collection", HttpStatusCode.BadRequest); 
            }

            if (!(await _sharedFileCollectionMemberValidator.ValidateAsync(sharedMembersToSave)).IsValid)
            {
                throw new ApiException("You submitted invalid shared members", HttpStatusCode.BadRequest);
            }
                
            var newlySavedMembers =
                await EntityFrameworkUtils.TryDbOperation(
                    () => _sharedFileCollectionMemberRepository.Create(sharedMembersToSave), _logger);
            
            if (newlySavedMembers?.IsSuccessful != true)
            {
                throw new ApiException(
                    "Failed to share file collection with new members"
                );
            }
            _logger.LogInformation(
                "Exiting {Action} successfully for correlationId {CorrelationId}",
                nameof(ShareFileCollectionAsync),
                correlationId
            );
            
            
            return newlySavedMembers.Data;
        }


        public async Task<FileCollection> GetFileCollectionWithContentsAsync(Guid fileCollectionId, Domain.Models.User currentUser)
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(GetFileCollectionWithContentsAsync),
                correlationId
            );

            var foundCollection = await EntityFrameworkUtils.TryDbOperation(
                () => _repository.GetOne(fileCollectionId, nameof(FileCollectionEntity.Documents)), _logger
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
                nameof(GetFileCollectionWithContentsAsync),
                correlationId
            );

            return foundCollection.Data;
        }

        public async Task<FileCollection> SaveFileCollectionAsync(
            FileCollectionSaveInput fileCollectionInput,
            Domain.Models.User currentUser
        )
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(SaveFileCollectionAsync),
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
                throw new ApiException($"{nameof(FileCollection)} is not valid", HttpStatusCode.BadRequest);
            }

            if (hasId)
            {
                _logger.LogInformation(
                    "Attempting to retrieve collection with id {CreatedCollectionId} for userId {UserId} and correlationId {CorrelationId}",
                    createdCollection.Id,
                    currentUser.Id,
                    correlationId
                );
                var foundOne = await EntityFrameworkUtils.TryDbOperation(() => _repository.GetOne((Guid)createdCollection.Id!), _logger);
                if (foundOne?.Data is null)
                {
                    throw new ApiException(
                        "Cannot find collection to update",
                        HttpStatusCode.BadRequest
                    );
                }

                if (
                    !createdCollection.ValidateAgainstOriginal(foundOne.Data)
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
                nameof(SaveFileCollectionAsync),
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
                nameof(SaveFileCollectionAsync),
                correlationId
            );
            return newlySavedCollection.Data.First();
        }

        public async Task<Guid> DeleteFileCollectionAsync(Guid collectionId, Domain.Models.User currentUser)
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(DeleteFileCollectionAsync),
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
                nameof(DeleteFileCollectionAsync),
                correlationId
            );
            return deletedId.Data.First();
        }

        public async Task<FlatFileDocumentPartialCollectionView> GetOneLayerFileDocPartialsAndCollectionsAsync(
            Domain.Models.User currentUser,
            Guid? collectionId = null
        )
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(GetOneLayerFileDocPartialsAndCollectionsAsync),
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
                        : _fileDocumentRepository.GetManyDocumentPartialsByCollectionIdAndUserId(
                            (Guid)currentUser.Id!,
                            (Guid)collectionId!,
                            nameof(FileDocumentEntity.MetaData)
                        ),
                _logger
            );

            await Task.WhenAll(collectionsJob, partialDocumentsJob);
            var collections = (await collectionsJob)?.Data ?? [];
            var partialDocuments = (await partialDocumentsJob)?.Data ?? [];

            _logger.LogInformation(
                "Exiting {Action} for correlationId {CorrelationId}",
                nameof(GetOneLayerFileDocPartialsAndCollectionsAsync),
                correlationId
            );

            return new FlatFileDocumentPartialCollectionView
            {
                Self = collections.FastArrayFirstOrDefault(x => x.Id == collectionId),
                FileCollections = collections.FastArrayWhere(x => x.Id != collectionId).ToArray(),
                FileDocuments = partialDocuments,
            };
        }
        
        
        private static SharedFileCollectionMember[] CreateSharedFileCollectionMembers(
            SharedFileCollectionMemberSaveInput sharedFileColInput,
            IReadOnlyCollection<SharedFileCollectionMember> existingSharedMembers,
            Guid collectionOwnerUserId)
        {
            var existingMemberIds = existingSharedMembers.FastArraySelect(x => x.UserId);
            return sharedFileColInput
                .MembersToShareTo
                .FastArrayWhere(x => x.UserId != collectionOwnerUserId && !existingMemberIds.Contains(x.UserId))
                .FastArraySelect(x => x.ToSharedFileCollectionMember(sharedFileColInput.CollectionId))
                .ToArray();
        }
    }
}
