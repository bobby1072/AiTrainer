using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AiTrainer.Web.Common.Exceptions;
using System.Net;
using FluentValidation;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Common.Extensions;
using System.ComponentModel.Design;
using AiTrainer.Web.Common.Models.ApiModels.Request;
namespace AiTrainer.Web.Domain.Services.File.Concrete
{
    internal class FileDocumentProcessingManager: BaseDomainService
    {
        private readonly ILogger<FileDocumentProcessingManager> _logger;
        private readonly IFileDocumentRepository _fileDocumentRepository;
        private readonly IValidator<FileDocument> _validator;
        private readonly IHttpContextAccessor _contextAccessor;
        public FileDocumentProcessingManager(
            IDomainServiceActionExecutor domainServiceActionExecutor,
            ILogger<FileDocumentProcessingManager> logger,
            IFileDocumentRepository fileDocumentRepository,
            IValidator<FileDocument> validator,
            IHttpContextAccessor contextAccessor
        )
        : base(domainServiceActionExecutor)
        {
            _logger = logger;
            _fileDocumentRepository = fileDocumentRepository;
            _validator = validator;
            _contextAccessor = contextAccessor;
        }

        public async Task<FileDocumentPartial> SaveDocument(FileDocumentFormInput formFile)
        {
            var currentUser = await _domainServiceActionExecutor.ExecuteAsync<IUserProcessingManager, Models.User?>(userService => userService.TryGetUserFromCache(_contextAccessor.HttpContext.GetAccessToken()));

            if(currentUser is null)
            {
                throw new ApiException("User is not logged in", HttpStatusCode.Unauthorized);
            }
            


            throw new NotImplementedException();
        } 
        private static FileTypeEnum GetFileType(IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (fileExtension != ".pdf" ||
                fileExtension != ".txt")
            {
                throw new ApiException($"{fileExtension} is not a valid file type", HttpStatusCode.BadRequest);
            }

            return fileExtension == ".pdf" ? FileTypeEnum.Pdf : FileTypeEnum.Text;
        }
    }
}
