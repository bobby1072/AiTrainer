using System.Net;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Models.Partials;
using AiTrainer.Web.Domain.Models.Views;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Utils;
using BT.Common.FastArray.Proto;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.File.Concrete
{
    internal sealed class FileCollectionProcessingManager : IFileCollectionProcessingManager
    {
        private readonly IFileCollectionRepository _repository;
        private readonly ILogger<FileCollectionProcessingManager> _logger;
        private readonly IValidator<FileCollection> _validator;
        private readonly IFileDocumentRepository _fileDocumentRepository;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly IRepository<
            SharedFileCollectionMemberEntity,
            Guid,
            SharedFileCollectionMember
        > _sharedFileCollectionMemberRepository;
        private readonly IValidator<
            IEnumerable<SharedFileCollectionMember>
        > _sharedFileCollectionMemberValidator;
        private readonly IRepository<UserEntity, Guid, Domain.Models.User> _userRepo;

        public FileCollectionProcessingManager(
            IFileCollectionRepository repository,
            ILogger<FileCollectionProcessingManager> logger,
            IValidator<FileCollection> validator,
            IFileDocumentRepository fileDocumentRepository,
            IRepository<
                SharedFileCollectionMemberEntity,
                Guid,
                SharedFileCollectionMember
            > sharedFileCollectionMemberRepository,
            IValidator<IEnumerable<SharedFileCollectionMember>> sharedFileCollectionMemberValidator,
            IRepository<UserEntity, Guid, Domain.Models.User> userRepo,
            IHttpContextAccessor? httpContextAccessor = null
        )
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _validator = validator;
            _fileDocumentRepository = fileDocumentRepository;
            _sharedFileCollectionMemberRepository = sharedFileCollectionMemberRepository;
            _sharedFileCollectionMemberValidator = sharedFileCollectionMemberValidator;
            _userRepo = userRepo;
            _fileDocumentRepository = fileDocumentRepository;
        }

        public async Task<Guid> UnshareFileCollectionAsync(
            RequiredGuidIdInput sharedFileMemberColId,
            Domain.Models.User currentUser
        )
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(UnshareFileCollectionAsync),
                correlationId
            );

            var foundSharedMember = await EntityFrameworkUtils.TryDbOperation(
                () =>
                    _sharedFileCollectionMemberRepository.GetOne(sharedFileMemberColId.Id),
                _logger
            );

            if (foundSharedMember?.IsSuccessful is false || foundSharedMember?.Data is null)
            {
                throw new ApiException("Could not find file collection with that id");
            }
            
            
            var foundfileCollection = await EntityFrameworkUtils.TryDbOperation(
                () =>
                    _repository.GetOne(
                        foundSharedMember.Data.CollectionId
                    ),
                _logger
            );

            if (foundfileCollection?.Data?.UserId != currentUser.Id)
            {
                throw new ApiException(
                    "You do not have permission to share this file collection",
                    HttpStatusCode.Unauthorized
                );
            }

            if (foundSharedMember.Data.ParentSharedMemberId is not null)
            {
                throw new ApiException("Cannot delete this member as it is not top level", HttpStatusCode.BadRequest);
            }
            
            var deletedId = await EntityFrameworkUtils.TryDbOperation(
                () => _sharedFileCollectionMemberRepository.Delete([(Guid)foundSharedMember.Data.Id!]),
                _logger
            );

            if (deletedId?.IsSuccessful != true)
            {
                throw new ApiException("Could not delete document");
            }
            _logger.LogInformation(
                "Exiting {Action} successfully for correlationId {CorrelationId}",
                nameof(UnshareFileCollectionAsync),
                correlationId
            );

            return deletedId.Data.First();
        }

        public async Task<IReadOnlyCollection<SharedFileCollectionMember>> ShareFileCollectionAsync(
            SharedFileCollectionMemberSaveInput sharedFileColInput,
            Domain.Models.User currentUser
        )
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();
            
            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(ShareFileCollectionAsync),
                correlationId
            );

            
            
            var emailInputs = sharedFileColInput
                .MembersToShareTo
                .FastArrayWhere(x => x is SharedFileCollectionSingleMemberEmailSaveInput)
                .FastArraySelect(x => (SharedFileCollectionSingleMemberEmailSaveInput)x)
                .ToArray();
            
            if (emailInputs.Length == 0)
            {
                var result  = await ShareFileCollectionAsync(
                    sharedFileColInput.CollectionId,
                    sharedFileColInput
                        .MembersToShareTo
                        .FastArrayWhere(x => x is SharedFileCollectionSingleMemberUserIdSaveInput)
                        .FastArraySelect(x => (SharedFileCollectionSingleMemberUserIdSaveInput)x)
                        .ToArray(),
                    currentUser
                );
                _logger.LogInformation(
                    "Exiting {Action} successfully for correlationId {CorrelationId}",
                    nameof(ShareFileCollectionAsync),
                    correlationId
                );
                return result; 
            }
            
            _logger.LogInformation("Attempting to retrieve users by email prior to sharing members for correlationId: {CorrelationId}",
                correlationId);
            
            var foundUsers = await 
                EntityFrameworkUtils
                    .TryDbOperation(() => 
                        _userRepo.GetMany<string>(emailInputs
                            .FastArraySelect(x => x.Email).ToArray(), nameof(UserEntity.Email)), _logger);
            
            
            if (foundUsers?.IsSuccessful is false || foundUsers?.Data is null)
            {
                throw new ApiException("Failed to retrieve file collection with that id");
            }
            var finalResult = await ShareFileCollectionAsync(
                sharedFileColInput.CollectionId,
                sharedFileColInput.MembersToShareTo.FastArraySelect(x =>
                    {
                        if (x is SharedFileCollectionSingleMemberEmailSaveInput foundEmailInput)
                        {
                            return new SharedFileCollectionSingleMemberUserIdSaveInput
                            {
                                UserId = (Guid)foundUsers.Data.Single(y => y.Email == foundEmailInput.Email).Id!,
                                CanCreateDocuments = x.CanCreateDocuments,
                                CanDownloadDocuments = x.CanDownloadDocuments,
                                CanRemoveDocuments = x.CanRemoveDocuments,
                                CanViewDocuments = x.CanViewDocuments
                            };
                        }
                        else
                        {
                            return new SharedFileCollectionSingleMemberUserIdSaveInput
                            {
                                UserId = ((SharedFileCollectionSingleMemberUserIdSaveInput)x).UserId,
                                CanCreateDocuments = x.CanCreateDocuments,
                                CanDownloadDocuments = x.CanDownloadDocuments,
                                CanRemoveDocuments = x.CanRemoveDocuments,
                                CanViewDocuments = x.CanViewDocuments
                            };
                        }
                    }).ToArray(), currentUser);
            
            _logger.LogInformation(
                "Exiting {Action} successfully for correlationId {CorrelationId}",
                nameof(ShareFileCollectionAsync),
                correlationId
            );

            return finalResult;
        }


        public async Task<FileCollection> GetFileCollectionWithContentsAsync(
            Guid fileCollectionId,
            Domain.Models.User currentUser
        )
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(GetFileCollectionWithContentsAsync),
                correlationId
            );

            var foundCollection = await EntityFrameworkUtils.TryDbOperation(
                () =>
                    _repository.GetOne(
                        fileCollectionId,
                        nameof(FileCollectionEntity.Documents),
                        nameof(FileCollectionEntity.SharedFileCollectionMembers)
                    ),
                _logger
            );

            if (foundCollection?.IsSuccessful is false || foundCollection?.Data is null)
            {
                throw new ApiException("Could not find file collection with that id");
            }

            if (
                foundCollection.Data.UserId != currentUser.Id
                && foundCollection.Data.SharedFileCollectionMembers?.CanAny(
                    (Guid)currentUser.Id!,
                    fileCollectionId,
                    SharedFileCollectionMemberPermission.DownloadDocuments
                ) != true
            )
            {
                throw new ApiException(
                    ExceptionConstants.Unauthorized,
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
                throw new ApiException(
                    $"{nameof(FileCollection)} is not valid",
                    HttpStatusCode.BadRequest
                );
            }

            if (hasId)
            {
                _logger.LogInformation(
                    "Attempting to retrieve collection with id {CreatedCollectionId} for userId {UserId} and correlationId {CorrelationId}",
                    createdCollection.Id,
                    currentUser.Id,
                    correlationId
                );
                var foundOne = await EntityFrameworkUtils.TryDbOperation(
                    () => _repository.GetOne((Guid)createdCollection.Id!),
                    _logger
                );
                if (foundOne?.Data is null)
                {
                    throw new ApiException(
                        "Cannot find collection to update",
                        HttpStatusCode.BadRequest
                    );
                }

                if (!createdCollection.ValidateAgainstOriginal(foundOne.Data))
                {
                    throw new ApiException("Cannot edit those fields", HttpStatusCode.BadRequest);
                }
                createdCollection.DateCreated = foundOne.Data.DateCreated.ToUniversalTime();
                createdCollection.DateModified = DateTime.UtcNow.ToUniversalTime();
            }
            IReadOnlyCollection<SharedFileCollectionMember> sharedMembers = [];
            if (createdCollection.ParentId is Guid foundParentId)
            {
                var foundSingleParent = await EntityFrameworkUtils.TryDbOperation(
                    () =>
                        _repository.GetOne(
                            foundParentId,
                            nameof(FileCollectionEntity.SharedFileCollectionMembers)
                        ),
                    _logger
                );
                if (foundSingleParent?.Data?.UserId != currentUser.Id)
                {
                    throw new ApiException(ExceptionConstants.Unauthorized, HttpStatusCode.Unauthorized);
                }

                sharedMembers =
                    foundSingleParent
                        .Data.SharedFileCollectionMembers?.FastArraySelect(x =>
                        {
                            x.ParentSharedMemberId = x.Id;
                            x.Id = null;
                            return x;
                        })
                        .ToArray() ?? [];
            }

            _logger.LogInformation(
                "{ActionName} attempting to {SaveMode} collection: {@CreatedCollection}",
                nameof(SaveFileCollectionAsync),
                hasId ? "update" : "create",
                createdCollection
            );
            var newlySavedCollection = await EntityFrameworkUtils.TryDbOperation(
                () =>
                    hasId ? _repository.Update([createdCollection])
                    : createdCollection.ParentId is not null
                        ? _repository.CreateWithSharedMembers(createdCollection, sharedMembers)
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

        public async Task<Guid> DeleteFileCollectionAsync(
            Guid collectionId,
            Domain.Models.User currentUser
        )
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
                throw new ApiException("Could not delete document");
            }
            _logger.LogInformation(
                "Exiting {Action} for correlationId {CorrelationId}",
                nameof(DeleteFileCollectionAsync),
                correlationId
            );
            return deletedId.Data.FirstOrDefault();
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
            var collections = await EntityFrameworkUtils.TryDbOperation(
                () =>
                    collectionId is null
                        ? _repository.GetTopLevelCollectionsForUser(
                            (Guid)currentUser.Id!
                        )
                        : _repository.GetManyCollectionsForUserIncludingSelf(
                            (Guid)collectionId!,
                            (Guid)currentUser.Id!
                        ),
                _logger
            );
            DbGetManyResult<FileDocumentPartial>? partialDocuments;

            var foundSharedMembers = collections
                ?.Data.FastArrayFirstOrDefault(x => x.Id == collectionId)
                ?.SharedFileCollectionMembers;
            if (
                collectionId is not null
                && foundSharedMembers?.CanAny(
                    (Guid)currentUser.Id!,
                    (Guid)collectionId,
                    SharedFileCollectionMemberPermission.ViewDocuments
                ) == true
            )
            {
                _logger.LogInformation("Shared member with correlationId: {CorrelationId} is accessing documents from collection with Id: {CollectionId}",
                    correlationId,
                    collectionId);
                partialDocuments = await EntityFrameworkUtils.TryDbOperation(
                    () =>
                        _fileDocumentRepository.GetManyDocumentPartialsByCollectionId(
                            (Guid)collectionId,
                            nameof(FileDocumentEntity.MetaData)
                        ),
                    _logger
                );
            }
            else
            {
                _logger.LogInformation("User with correlationId: {CorrelationId} is accessing their own documents from collection with Id: {CollectionId}",
                    correlationId,
                    collectionId);
                partialDocuments = await EntityFrameworkUtils.TryDbOperation(
                    () =>
                        _fileDocumentRepository.GetManyDocumentPartialsByCollectionIdAndUserId(
                            (Guid)currentUser.Id!,
                            collectionId,
                            nameof(FileDocumentEntity.MetaData)
                        ),
                    _logger
                );
            }

            _logger.LogInformation(
                "Exiting {Action} for correlationId {CorrelationId}",
                nameof(GetOneLayerFileDocPartialsAndCollectionsAsync),
                correlationId
            );

            return new FlatFileDocumentPartialCollectionView
            {
                Self = collections?.Data.FastArrayFirstOrDefault(x => x.Id == collectionId),
                FileCollections =
                    collections?.Data.FastArrayWhere(x => x.Id != collectionId).ToArray() ?? [],
                FileDocuments = partialDocuments?.Data ?? [],
            };
        }
        private async Task<IReadOnlyCollection<SharedFileCollectionMember>> ShareFileCollectionAsync(
            Guid collectionId,
            IReadOnlyCollection<SharedFileCollectionSingleMemberUserIdSaveInput> sharedFileColInput,
            Domain.Models.User currentUser
        )
        {
            if (sharedFileColInput.Count == 0)
            {
                return [];
            }
            
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();


            var foundCollection = await EntityFrameworkUtils.TryDbOperation(
                () =>
                    _repository.GetCollectionWithChildren(
                        collectionId,
                        nameof(FileCollectionEntity.SharedFileCollectionMembers)
                    ),
                _logger
            );
            if (foundCollection?.IsSuccessful != true || foundCollection.Data.Count == 0)
            {
                throw new ApiException("Failed to retrieve file collection with that id");
            }
            var mainCollection = foundCollection.Data.FastArrayFirstOrDefault(x => x.Id == collectionId) ?? throw new ApiException("Failed to retrieve file collection with that id");

            if (mainCollection.UserId != currentUser.Id)
            {
                throw new ApiException(
                    "You do not have permission to share this file collection",
                    HttpStatusCode.Unauthorized
                );
            }

            var sharedMembersToSave = CreateSharedFileCollectionMembers(
                collectionId,
                foundCollection.Data,
                sharedFileColInput
            );

            if (sharedMembersToSave.Length == 0)
            {
                _logger.LogInformation(
                    "No members to share for correlationId: {CorrelationId}",
                    correlationId
                );
                return [];
            }

            if (
                sharedMembersToSave.FastArrayWhere(x => x.ParentSharedMemberId == null).Count()
                    + (mainCollection.SharedFileCollectionMembers?.Count ?? 0)
                > 30
            )
            {
                throw new ApiException(
                    "you cannot share more than 30 users on a file collection",
                    HttpStatusCode.BadRequest
                );
            }

            if (
                !(
                    await _sharedFileCollectionMemberValidator.ValidateAsync(sharedMembersToSave)
                ).IsValid
            )
            {
                throw new ApiException(
                    "You submitted invalid shared members",
                    HttpStatusCode.BadRequest
                );
            }

            var newlySavedMembers = await EntityFrameworkUtils.TryDbOperation(
                () => _sharedFileCollectionMemberRepository.Create(sharedMembersToSave),
                _logger
            );

            if (newlySavedMembers?.IsSuccessful != true)
            {
                throw new ApiException("Failed to share file collection with new members");
            }

            _logger.LogInformation(
                "Successfully shared document to {MemberCount} members with collectionId: {CollectionId} and correlationId: {CorrelationId}",
                newlySavedMembers.Data.Count,
                collectionId,
                correlationId
            );
            return newlySavedMembers.Data;
        }

        private static SharedFileCollectionMember[] CreateSharedFileCollectionMembers(
            Guid mainFileCollectionId,
            IReadOnlyCollection<FileCollection> fileCollectionWithChildrenAndMembers,
            IReadOnlyCollection<SharedFileCollectionSingleMemberUserIdSaveInput> newMembersInput
        )
        {
            var collectionOwnerIds = fileCollectionWithChildrenAndMembers.FastArraySelect(x => x.UserId).ToArray();
            
            var existingTopLevelMemberIds = fileCollectionWithChildrenAndMembers
                .SelectMany(x => x.SharedFileCollectionMembers ?? [])
                .FastArrayWhere(x => x.ParentSharedMemberId is null)
                .FastArraySelect(x => x.UserId)
                .ToArray();
            
            var actualTopLevelMembersToCreate = newMembersInput
                .FastArrayWhere(x =>
                    !collectionOwnerIds.Contains(x.UserId) && !existingTopLevelMemberIds.Contains(x.UserId)
                )
                .ToArray();
            var firstMembers = actualTopLevelMembersToCreate
                .FastArraySelect(x => x.ToNewSharedFileCollectionMember(mainFileCollectionId)).ToArray();
            
            var newMembersList = new List<SharedFileCollectionMember>();
            newMembersList.AddRange(firstMembers);
            foreach (var fileCol in OrderFileCollectionsHierarchically(fileCollectionWithChildrenAndMembers, mainFileCollectionId))
            {
                if (fileCol.Id == mainFileCollectionId)
                {
                    continue;
                }
                else
                {
                    newMembersList
                        .AddRange(actualTopLevelMembersToCreate
                            .FastArraySelect(x => x.ToNewSharedFileCollectionMember(
                                (Guid)fileCol.Id!,
                                newMembersList.FastArrayFirstOrDefault(y => y.CollectionId == fileCol.ParentId)!.Id
                            )).ToArray());
                }
            }
            
            
            return newMembersList.ToArray();
        }
        private static List<FileCollection> OrderFileCollectionsHierarchically(
            IEnumerable<FileCollection> collections,
            Guid topLevelCollectionId)
        {
            var lookup = collections.ToLookup(fc => fc.ParentId);
            var map = collections.ToDictionary(fc => fc.Id!.Value);
            var result = new List<FileCollection>();

            if (!map.TryGetValue(topLevelCollectionId, out var root))
                throw new ArgumentException("Top-level collection ID not found in the provided list.");

            void AddWithChildren(FileCollection parent)
            {
                result.Add(parent);
                foreach (var child in lookup[parent.Id])
                {
                    AddWithChildren(child);
                }
            }

            AddWithChildren(root);
            return result;
        }
    }
}
