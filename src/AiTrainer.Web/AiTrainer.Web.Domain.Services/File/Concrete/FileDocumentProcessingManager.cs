using System.Net;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.File.Concrete
{
    public class FileDocumentProcessingManager : BaseDomainService, IFileDocumentProcessingManager
    {
        private readonly ILogger<FileDocumentProcessingManager> _logger;
        private readonly IFileDocumentRepository _fileDocumentRepository;
        private readonly IValidator<FileDocument> _validator;

        public FileDocumentProcessingManager(
            IDomainServiceActionExecutor domainServiceActionExecutor,
            IApiRequestHttpContextService apiRequestService,
            ILogger<FileDocumentProcessingManager> logger,
            IFileDocumentRepository fileDocumentRepository,
            IValidator<FileDocument> validator
        )
            : base(domainServiceActionExecutor, apiRequestService)
        {
            _logger = logger;
            _fileDocumentRepository = fileDocumentRepository;
            _validator = validator;
        }

        public async Task<FileDocument> UploadFile(
            FileDocumentSaveFormInput fileDocumentSaveFormInput
        )
        {
            var correlationId = _apiRequestHttpContextService.CorrelationId;

            _logger.LogInformation(
                "Entering {Action} for correlationId {CorrelationId}",
                nameof(UploadFile),
                correlationId
            );

            var foundCachedUser =
                await _domainServiceActionExecutor.ExecuteAsync<
                    IUserProcessingManager,
                    Models.User?
                >(userServ =>
                    userServ.TryGetUserFromCache(_apiRequestHttpContextService.AccessToken)
                ) ?? throw new ApiException("Can't find user", HttpStatusCode.Unauthorized);

            var newFileDoc = await fileDocumentSaveFormInput.ToDocumentModel(
                (Guid)foundCachedUser.Id!
            );

            var isValid = await _validator.ValidateAsync(newFileDoc);

            if (!isValid.IsValid)
            {
                throw new ApiException("Invalid file document", HttpStatusCode.BadRequest);
            }

            _logger.LogInformation(
                "Exiting {Action} for correlationId {CorrelationId}",
                nameof(UploadFile),
                correlationId
            );
            throw new NotImplementedException();
        }
    }
}
