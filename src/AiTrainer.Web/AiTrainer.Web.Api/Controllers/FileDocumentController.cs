using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Common.Models.ApiModels.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Partials;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.File.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    [Authorize]
    [RequireUserLogin]
    public class FileDocumentController : BaseController
    {
        public FileDocumentController(IDomainServiceActionExecutor actionExecutor)
            : base(actionExecutor) { }
        [HttpPost("Upload")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<Outcome<FileDocumentPartial>>> Upload(
            [FromForm] Guid? collectionId,
            [FromForm] IFormFile file
        )
        {
            var formInput = new FileDocumentSaveFormInput
            {
                CollectionId = collectionId,
                FileToCreate = file
            };
            var result = await _actionExecutor.ExecuteAsync<
                IFileDocumentProcessingManager,
                FileDocumentPartial
            >(service => service.UploadFileDocument(formInput));

            return new Outcome<FileDocumentPartial> { Data = result };
        }

        [HttpPost("Delete")]
        public async Task<ActionResult<Outcome<Guid>>> Delete([FromBody] RequiredIdInput input)
        {
            var result = await _actionExecutor.ExecuteAsync<IFileDocumentProcessingManager, Guid>(
                service => service.DeleteFileDocument(input.Id)
            );

            return new Outcome<Guid> { Data = result };
        }
    }
}
