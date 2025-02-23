using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Common.Models.ApiModels.Request;
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
using AiTrainer.Web.Domain.Services.File.Models;

namespace AiTrainer.Web.Domain.Services.File.Concrete
{
    internal class FileDocumentProcessingManager : IFileDocumentProcessingManager
    {
        private readonly ILogger<FileDocumentProcessingManager> _logger;
        private readonly IFileDocumentRepository _fileDocumentRepository;
        private readonly IValidator<FileDocument> _validator;
        private readonly IFileCollectionRepository _fileCollectionRepository;
        private readonly IFileCollectionFaissRepository _fileCollectionFaissRepository;
        private readonly IFileCollectionFaissSyncBackgroundJobQueue _faissSyncBackgroundJobQueue;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public FileDocumentProcessingManager(
            ILogger<FileDocumentProcessingManager> logger,
            IFileDocumentRepository fileDocumentRepository,
            IFileCollectionFaissRepository fileCollectionFaissRepository,
            IValidator<FileDocument> validator,
            IFileCollectionRepository fileCollectionRepository,
            IFileCollectionFaissSyncBackgroundJobQueue faissSyncBackgroundJobQueue,
            IHttpContextAccessor? httpContextAccessor = null
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _fileDocumentRepository = fileDocumentRepository;
            _validator = validator;
            _fileCollectionFaissRepository = fileCollectionFaissRepository;
            _fileCollectionRepository = fileCollectionRepository;
            _faissSyncBackgroundJobQueue = faissSyncBackgroundJobQueue;
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
                () => _fileDocumentRepository.GetOne(documentId, (Guid)currentUser.Id!),
                _logger
            );

            if (foundDocument?.Data?.UserId != (Guid)currentUser.Id!)
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

            if (newFileDoc.CollectionId is not null)
            {
                var foundParent = await EntityFrameworkUtils.TryDbOperation(
                    () => _fileCollectionRepository.GetOne((Guid)newFileDoc.CollectionId!),
                    _logger
                );

                if (foundParent?.Data?.UserId != currentUser.Id)
                {
                    throw new ApiException(ExceptionConstants.Unauthorized, HttpStatusCode.Unauthorized);
                }
            }

            var createdFile = await EntityFrameworkUtils.TryDbOperation(
                async () => newFileDoc.FileType == FileTypeEnum.Pdf ? await _fileDocumentRepository.Create(newFileDoc, await FileDocumentMetaDataHelper.GetFromFormFile(fileDocumentSaveFormInput.FileToCreate, (Guid)newFileDoc.Id!)) : await _fileDocumentRepository.Create([newFileDoc]),
                _logger
            );

            if (createdFile?.IsSuccessful != true)
            {
                throw new ApiException("Invalid file document", HttpStatusCode.BadRequest);
            }

            _logger.LogInformation(
                "Exiting {Action} for correlationId {CorrelationId}",
                nameof(UploadFileDocument),
                correlationId
            );
            return createdFile.Data.First().ToPartial();
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

            if (documentToDelete?.Data?.UserId != (Guid)currentUser.Id!)
            {
                throw new ApiException(ExceptionConstants.Unauthorized, HttpStatusCode.Unauthorized);
            }

            var deletedJobResult = await EntityFrameworkUtils.TryDbOperation(
                () => _fileCollectionFaissRepository.DeleteDocumentAndStoreAndUnsyncDocuments(documentToDelete.Data),
                _logger
            );

            if (deletedJobResult?.IsSuccessful != true)
            {
                throw new ApiException(
                    "Could not delete document"
                );
            }
            _logger.LogInformation(
                "Exiting {Action} for correlationId {CorrelationId}",
                nameof(UploadFileDocument),
                correlationId
            );

            await _faissSyncBackgroundJobQueue.Enqueue(new FileCollectionFaissSyncBackgroundJob
            { User = currentUser, CollectionId = documentToDelete.Data.CollectionId });

            return (Guid)documentToDelete.Data.Id!;
        }
    }
}
