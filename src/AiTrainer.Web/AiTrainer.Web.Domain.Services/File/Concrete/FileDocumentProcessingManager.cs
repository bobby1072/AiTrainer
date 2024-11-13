using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;


namespace AiTrainer.Web.Domain.Services.File.Concrete
{
    public class FileDocumentProcessingManager: BaseDomainService, IFileDocumentProcessingManager
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
        private static (string, FileTypeEnum) GetFileType(IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLower();


            return fileExtension switch
            {
                ".pdf" => (Path.GetFileNameWithoutExtension(file.FileName), FileTypeEnum.Pdf),
                ".txt" => (Path.GetFileNameWithoutExtension(file.FileName), FileTypeEnum.Text),
                _ => throw new ApiException($"{fileExtension} is not a valid file type", HttpStatusCode.BadRequest)
            };
        }
    }
}
