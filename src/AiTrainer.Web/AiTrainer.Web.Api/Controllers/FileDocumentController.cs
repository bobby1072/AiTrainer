using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Common.Models.ApiModels.Response;
using AiTrainer.Web.Domain.Models.Partials;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.File.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    [RequireUserLogin]
    public class FileDocumentController : BaseController
    {
        public FileDocumentController(IDomainServiceActionExecutor actionExecutor)
            : base(actionExecutor) { }

        [HttpPost("Upload")]
        public async Task<ActionResult<Outcome<FileDocumentPartial>>> Upload(
            [FromForm] FileDocumentSaveFormInput formInput
        )
        {
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
