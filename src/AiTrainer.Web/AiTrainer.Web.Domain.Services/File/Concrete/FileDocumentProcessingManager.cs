using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Models.Helpers;
using AiTrainer.Web.Domain.Models.Partials;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using AiTrainer.Web.Domain.Services.File.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Services.ChatGpt.Abstract;
using AiTrainer.Web.Persistence.Entities;

namespace AiTrainer.Web.Domain.Services.File.Concrete
{
    internal sealed class FileDocumentProcessingManager : IFileDocumentProcessingManager
    {
        private readonly ILogger<FileDocumentProcessingManager> _logger;
        private readonly IFileDocumentRepository _fileDocumentRepository;
        private readonly IValidator<FileDocument> _validator;
        private readonly IFileCollectionRepository _fileCollectionRepository;
        private readonly IFileCollectionFaissSyncBackgroundJobQueue _faissSyncBackgroundJobQueue;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly IValidator<PotentialDocumentEditChatRawQueryInput> _potentialDocumentEditChatRawQueryInputValidator;
        private readonly IChatGptQueryProcessingManager _chatGptQueryProcessingManager;
        public FileDocumentProcessingManager(
            ILogger<FileDocumentProcessingManager> logger,
            IFileDocumentRepository fileDocumentRepository,
            IValidator<FileDocument> validator,
            IFileCollectionRepository fileCollectionRepository,
            IFileCollectionFaissSyncBackgroundJobQueue faissSyncBackgroundJobQueue,
            IValidator<PotentialDocumentEditChatRawQueryInput> potentialDocumentEditChatRawQueryInputValidator,
            IChatGptQueryProcessingManager chatGptQueryProcessingManager,
            IHttpContextAccessor? httpContextAccessor = null
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _fileDocumentRepository = fileDocumentRepository;
            _validator = validator;
            _fileCollectionRepository = fileCollectionRepository;
            _potentialDocumentEditChatRawQueryInputValidator = potentialDocumentEditChatRawQueryInputValidator;
            _chatGptQueryProcessingManager = chatGptQueryProcessingManager;
            _faissSyncBackgroundJobQueue = faissSyncBackgroundJobQueue;
        }

        public async Task<FileDocument> PotentialDocumentEditChatQuery(
            PotentialDocumentEditChatRawQueryInput input,
            Domain.Models.User currentUser,
            CancellationToken cancellationToken = default
        )
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(PotentialDocumentEditChatQuery),
                correlationId
            );
            var validationResult = await _potentialDocumentEditChatRawQueryInputValidator.ValidateAsync(input, cancellationToken);
            
            if (!validationResult.IsValid)
            {
                throw new ApiException("Invalid query input", HttpStatusCode.BadRequest);
            }
            
            var foundDocument = await EntityFrameworkUtils.TryDbOperation(
                () => _fileDocumentRepository.GetOne(input.FileDocumentId),
                _logger
            );
            if (foundDocument?.Data is null)
            {
                throw new ApiException("Failed to retrieve file document");
            }

            if (foundDocument.Data.FileType != FileTypeEnum.Text)
            {
                throw new ApiException("File type not supported", HttpStatusCode.BadRequest);
            }
            
            var gptResult = await _chatGptQueryProcessingManager.ChatGptQuery(
                    new EditFileDocumentQueryInput
                    {
                        ChangeRequest = input.ChangeRequest,
                        FileDocumentToChange = foundDocument.Data,
                    },
                    currentUser,
                    cancellationToken
                );


            foundDocument.Data.FileData = Encoding.UTF8.GetBytes(gptResult);
            
            return foundDocument.Data;
        }
        public async Task<FileDocument> GetFileDocumentForDownload(Guid documentId, Domain.Models.User currentUser)
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(GetFileDocumentForDownload),
                correlationId
            );
            var foundDocument = await EntityFrameworkUtils.TryDbOperation(
                () => _fileDocumentRepository.GetOne(documentId),
                _logger
            );
            if (foundDocument?.Data is null)
            {
                throw new ApiException("Failed to retrieve file document");
            }
            IReadOnlyCollection<SharedFileCollectionMember> sharedFileCollectionMembers = [];

            if (foundDocument.Data.CollectionId is Guid foundCollectionId)
            {
                var foundCollection = await EntityFrameworkUtils.TryDbOperation(
                    () => _fileCollectionRepository.GetOne(foundCollectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)),
                    _logger
                );
                if (foundCollection?.Data is null)
                {
                    throw new ApiException("Failed to retrieve parent collection");
                }
                sharedFileCollectionMembers = foundCollection?.Data?.SharedFileCollectionMembers ?? [];
            }
            if (foundDocument.Data?.UserId != (Guid)currentUser.Id! && !(foundDocument.Data?.CollectionId is not null && sharedFileCollectionMembers.CanAny(
                    (Guid)currentUser.Id!, (Guid)foundDocument.Data.CollectionId!, SharedFileCollectionMemberPermission.DownloadDocuments)))
            {
                throw new ApiException(ExceptionConstants.Unauthorized, HttpStatusCode.Unauthorized);
            }
            _logger.LogInformation(
                "Exiting {Action} for correlationId {CorrelationId}",
                nameof(GetFileDocumentForDownload),
                correlationId
            );

            return foundDocument.Data;
        }

        public async Task<FileDocumentPartial> UploadFileDocument(
            FileDocumentSaveFormInput fileDocumentSaveFormInput,
            Domain.Models.User currentUser
        )
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(UploadFileDocument),
                correlationId
            );


            var newFileDoc = await fileDocumentSaveFormInput.ToDocumentModel(
                (Guid)currentUser.Id!
            );
            newFileDoc.ApplyCreationDefaults();
            var isValid = await _validator.ValidateAsync(newFileDoc);

            if (!isValid.IsValid)
            {
                throw new ApiException("Invalid file document", HttpStatusCode.BadRequest);
            }
            FileCollection? foundParentCollection = null;
            if (newFileDoc.CollectionId is not null)
            {
                var foundParent = await EntityFrameworkUtils.TryDbOperation(
                    () => _fileCollectionRepository.GetOne((Guid)newFileDoc.CollectionId!, nameof(FileCollectionEntity.SharedFileCollectionMembers)),
                    _logger
                );
                
                if (foundParent?.Data?.UserId != currentUser.Id && 
                    foundParent?.Data?.SharedFileCollectionMembers?.CanAny((Guid)currentUser.Id!, (Guid)newFileDoc.CollectionId!, SharedFileCollectionMemberPermission.CreateDocuments) != true)
                {
                    throw new ApiException(ExceptionConstants.Unauthorized, HttpStatusCode.Unauthorized);
                }
                foundParentCollection = foundParent.Data;
            }

            var createdFile = await EntityFrameworkUtils.TryDbOperation(
                async () => newFileDoc.FileType == FileTypeEnum.Pdf ? await _fileDocumentRepository.Create(newFileDoc, await FileDocumentMetaDataHelper.GetFromFormFile(fileDocumentSaveFormInput.FileToCreate, (Guid)newFileDoc.Id!)) : await _fileDocumentRepository.Create([newFileDoc]),
                _logger
            );

            if (createdFile?.IsSuccessful != true)
            {
                throw new ApiException("Failed to save file document");
            }

            if (foundParentCollection is { AutoFaissSync: true } ||
                (newFileDoc.CollectionId == null && currentUser.GlobalFileCollectionConfig?.AutoFaissSync == true))
            {
                await _faissSyncBackgroundJobQueue.EnqueueAsync(new FileCollectionFaissSyncBackgroundJob
                {
                    CurrentUser = currentUser,
                    CollectionId = newFileDoc.CollectionId,
                    RetryOverride = false
                });
            }

            _logger.LogInformation(
                "Exiting {Action} for correlationId {CorrelationId}",
                nameof(UploadFileDocument),
                correlationId
            );
            return createdFile.Data.FirstOrDefault()?.ToPartial() ?? throw new ApiException("Failed to save file document");
        }
        public async Task<Guid> DeleteFileDocument(Guid documentId, Domain.Models.User currentUser)
        {
            var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(UploadFileDocument),
                correlationId
            );

            var documentToDelete =
                await EntityFrameworkUtils.TryDbOperation(
                    () => _fileDocumentRepository.GetOne(documentId),
                    _logger
                );

            if (documentToDelete?.Data is null)
            {
                throw new ApiException("Failed to retrieve document");
            }

            if (documentToDelete.Data.CollectionId is not null)
            {
                var foundParent = await EntityFrameworkUtils.TryDbOperation(
                    () => _fileCollectionRepository.GetOne((Guid)documentToDelete.Data.CollectionId!, nameof(FileCollectionEntity.SharedFileCollectionMembers)),
                    _logger
                );
                
                if (foundParent?.Data?.UserId != currentUser.Id && 
                    foundParent?.Data?.SharedFileCollectionMembers?.CanAny((Guid)currentUser.Id!, (Guid)documentToDelete.Data.CollectionId!, SharedFileCollectionMemberPermission.RemoveDocuments) != true)
                {
                    throw new ApiException(ExceptionConstants.Unauthorized, HttpStatusCode.Unauthorized);
                }
            }
            else if (documentToDelete.Data.UserId != (Guid)currentUser.Id!)
            {
                throw new ApiException(ExceptionConstants.Unauthorized, HttpStatusCode.Unauthorized);
            }
            
            
            var deletedJobResult = await EntityFrameworkUtils.TryDbOperation(
                () => _fileDocumentRepository.Delete([documentToDelete.Data]),
                _logger
            );

            if (deletedJobResult?.Data is null || deletedJobResult.Data.Count < 1)
            {
                throw new ApiException(
                    "Could not delete document"
                );
            }

            await _faissSyncBackgroundJobQueue.EnqueueAsync(new FileCollectionFaissRemoveDocumentsBackgroundJob
            {
                CurrentUser = currentUser,
                CollectionId = documentToDelete.Data.CollectionId
            });
            _logger.LogInformation(
                "Exiting {Action} for correlationId {CorrelationId}",
                nameof(UploadFileDocument),
                correlationId
            );
            

            return (Guid)documentToDelete.Data.Id!;
        }
    }
}
