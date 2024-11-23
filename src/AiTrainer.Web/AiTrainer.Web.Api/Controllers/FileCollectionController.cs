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
    [RequireUserLogin]
    public class FileCollectionController : BaseController
    {
        public FileCollectionController(IDomainServiceActionExecutor actionExecutor)
            : base(actionExecutor) { }

        [HttpPost("Save")]
        public async Task<ActionResult<Outcome<FileCollection>>> SaveCollection(
            [FromBody] FileCollectionSaveInput fileCollection
        )
        {
            var result = await _actionExecutor.ExecuteAsync<
                IFileCollectionProcessingManager,
                FileCollection
            >(service => service.SaveFileCollection(fileCollection));

            return new Outcome<FileCollection> { Data = result };
        }

        [HttpPost("GetOneLayer")]
        public async Task<ActionResult<Outcome<FlatFileDocumentPartialCollection>>> GetOneLayer(
            [FromBody] OptionalIdInput input
        )
        {
            var result = await _actionExecutor.ExecuteAsync<
                IFileCollectionProcessingManager,
                FlatFileDocumentPartialCollection
            >(service => service.GetOneLayerFileDocPartialsAndCollections(input.Id));

            return new Outcome<FlatFileDocumentPartialCollection> { Data = result };
        }

        [HttpPost("Delete")]
        public async Task<ActionResult<Outcome<Guid>>> Delete([FromBody] RequiredIdInput input)
        {
            var result = await _actionExecutor.ExecuteAsync<IFileCollectionProcessingManager, Guid>(
                service => service.DeleteFileCollection(input.Id)
            );

            return new Outcome<Guid> { Data = result };
        }
    }
}
