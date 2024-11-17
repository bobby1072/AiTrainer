using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Partials;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.File.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    [Authorize]
    [RequireLogin]
    public class FileDocumentController: BaseController
    {
        public FileDocumentController(IDomainServiceActionExecutor actionExecutor): base(actionExecutor) { }
        [HttpPost("Upload")]
        public async Task<ActionResult<Outcome<FileDocumentPartial>>> Upload([FromForm] FileDocumentSaveFormInput formInput)
        {
            var result = await _actionExecutor.ExecuteAsync<IFileDocumentProcessingManager, FileDocumentPartial>(sevrice => sevrice.UploadFile(formInput));

            return new Outcome<FileDocumentPartial> { IsSuccess = true, Data = result };
        }
    }
}
