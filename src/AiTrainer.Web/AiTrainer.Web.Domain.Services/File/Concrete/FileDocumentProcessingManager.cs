using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Models.Partials;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AiTrainer.Web.Domain.Services.File.Concrete
{
    public class FileDocumentProcessingManager : IFileDocumentProcessingManager
    {
        private readonly ILogger<FileDocumentProcessingManager> _logger;
        private readonly IFileDocumentRepository _fileDocumentRepository;
        private readonly IValidator<FileDocument> _validator;
        private readonly IFileCollectionRepository _fileCollectionRepository;
        private readonly IUserProcessingManager _userProcessingManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FileDocumentProcessingManager(
            IUserProcessingManager userProcessingManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<FileDocumentProcessingManager> logger,
            IFileDocumentRepository fileDocumentRepository,
            IValidator<FileDocument> validator,
            IFileCollectionRepository fileCollectionRepository
        )
        {
            _userProcessingManager = userProcessingManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _fileDocumentRepository = fileDocumentRepository;
            _validator = validator;
            _fileCollectionRepository = fileCollectionRepository;
        }

        public async Task<FileDocument> GetFileDocumentForDownload(Guid documentId)
        {
            var correlationId = _httpContextAccessor.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(GetFileDocumentForDownload),
                correlationId
            );

            var foundCachedUser =
                await _userProcessingManager.TryGetUserFromCache(_httpContextAccessor.HttpContext.GetAccessToken())?? throw new ApiException("Can't find user", HttpStatusCode.Unauthorized);

            var foundDocument = await EntityFrameworkUtils.TryDbOperation(
                () => _fileDocumentRepository.GetOne(documentId, (Guid)foundCachedUser.Id!),
                _logger
            );

            if (foundDocument?.IsSuccessful is not true || foundDocument.Data is null)
            {
                throw new ApiException("Cannot find document", HttpStatusCode.NotFound);
            }

            _logger.LogInformation(
                "Exiting {Action} for correlationId {CorrelationId}",
                nameof(GetFileDocumentForDownload),
                correlationId
            );

            return foundDocument.Data;
        }

        public async Task<FileDocumentPartial> UploadFileDocument(
            FileDocumentSaveFormInput fileDocumentSaveFormInput
        )
        {
            var correlationId = _httpContextAccessor.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(UploadFileDocument),
                correlationId
            );

            var foundCachedUser =
                await _userProcessingManager.TryGetUserFromCache(_httpContextAccessor.HttpContext.GetAccessToken())?? throw new ApiException("Can't find user", HttpStatusCode.Unauthorized);

            var newFileDoc = await fileDocumentSaveFormInput.ToDocumentModel(
                (Guid)foundCachedUser.Id!
            );

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

                if (foundParent?.Data?.UserId != foundCachedUser.Id)
                {
                    throw new ApiException("Invalid file document", HttpStatusCode.BadRequest);
                }
            }

            var createdFile = await EntityFrameworkUtils.TryDbOperation(
                () => _fileDocumentRepository.Create([newFileDoc]),
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

        public async Task<Guid> DeleteFileDocument(Guid documentId)
        {
            var correlationId = _httpContextAccessor.HttpContext?.GetCorrelationId();

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(UploadFileDocument),
                correlationId
            );

            var foundCachedUser =
                await _userProcessingManager.TryGetUserFromCache(_httpContextAccessor.HttpContext.GetAccessToken())?? throw new ApiException("Can't find user", HttpStatusCode.Unauthorized);

            var deletedId = await EntityFrameworkUtils.TryDbOperation(
                () => _fileDocumentRepository.Delete(documentId, (Guid)foundCachedUser.Id!),
                _logger
            );

            if (deletedId?.IsSuccessful != true)
            {
                throw new ApiException(
                    "Could not delete document",
                    HttpStatusCode.InternalServerError
                );
            }
            _logger.LogInformation(
                "Exiting {Action} for correlationId {CorrelationId}",
                nameof(UploadFileDocument),
                correlationId
            );
            return deletedId.Data.First();
        }
    }
}
