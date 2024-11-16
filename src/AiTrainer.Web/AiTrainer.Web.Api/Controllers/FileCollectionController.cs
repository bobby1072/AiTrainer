using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.File.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    [Authorize]
    [RequireLogin]
    public class FileCollectionController: BaseController
    {
        public FileCollectionController(IDomainServiceActionExecutor actionExecutor): base(actionExecutor) { }
        [HttpPost("save")]
        public async Task<ActionResult<Outcome<FileCollection>>> SaveCollection([FromBody] FileCollectionSaveInput fileCollection)
        {
            var result = await _actionExecutor.ExecuteAsync<IFileCollectionProcessingManager, FileCollection>(service => service.SaveFileCollection(fileCollection));

            return new Outcome<FileCollection> { IsSuccess = true, Data = result};
        }
    }
}
