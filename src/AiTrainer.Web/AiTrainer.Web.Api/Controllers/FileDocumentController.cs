using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Models.Partials;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.File.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    [RequireUserLogin]
    public sealed class FileDocumentController : BaseController
    {
        private readonly ILogger<FileDocumentController> _logger;

        public FileDocumentController(
            IHttpDomainServiceActionExecutor actionExecutor,
            ILogger<FileDocumentController> logger
        )
            : base(actionExecutor)
        {
            _logger = logger;
        }

        [HttpPost("Download")]
        public async Task<IActionResult> Download([FromBody] RequiredGuidIdInput input)
        {
            var currentUser = await GetCurrentUser();

            var result = await _actionExecutor.ExecuteAsync<
                IFileDocumentProcessingManager,
                FileDocument
            >(service => service.GetFileDocumentForDownload(input.Id, currentUser), nameof(IFileDocumentProcessingManager.GetFileDocumentForDownload));

            var memoryStream = new MemoryStream(result.FileData);
            return File(memoryStream, result.GetMimeType(), result.FileName);
        }

        [HttpPost("Upload")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<Outcome<FileDocumentPartial>>> Upload(
            [FromForm] Guid? collectionId,
            [FromForm] string? fileDescription,
            [FromForm] IFormFile file
        )
        {
            var formInput = new FileDocumentSaveFormInput
            {
                CollectionId = collectionId,
                FileToCreate = file,
                FileDescription = fileDescription,
            };
            
            var currentUser = await GetCurrentUser();

            var result = await _actionExecutor.ExecuteAsync<
                IFileDocumentProcessingManager,
                FileDocumentPartial
            >(service => service.UploadFileDocument(formInput, currentUser), nameof(IFileDocumentProcessingManager.UploadFileDocument));

            return new Outcome<FileDocumentPartial> { Data = result };
        }

        [HttpPost("Delete")]
        public async Task<ActionResult<Outcome<Guid>>> Delete([FromBody] RequiredGuidIdInput input)
        {
            var currentUser = await GetCurrentUser();

            var result = await _actionExecutor.ExecuteAsync<IFileDocumentProcessingManager, Guid>(
                service => service.DeleteFileDocument(input.Id, currentUser), nameof(IFileDocumentProcessingManager.DeleteFileDocument)
            );

            return new Outcome<Guid> { Data = result };
        }
    }
}
