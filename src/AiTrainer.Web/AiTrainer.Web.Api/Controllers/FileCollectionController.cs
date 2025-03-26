using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.File.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    [RequireUserLogin]
    public class FileCollectionController : BaseController
    {
        public FileCollectionController(IDomainServiceActionExecutor actionExecutor)
            : base(actionExecutor) { }

        [HttpPost("Download")]
        public async Task<IActionResult> Download([FromBody] RequiredGuidIdInput input)
        {
            var currentUser = await GetCurrentUser();
            var result = await _actionExecutor.ExecuteAsync<
                IFileCollectionProcessingManager,
                FileCollection
            >(service => service.GetFileCollectionWithContents(input.Id, currentUser));

            await using var memoryStream = new MemoryStream();
            using (
                var archive = new System.IO.Compression.ZipArchive(
                    memoryStream,
                    System.IO.Compression.ZipArchiveMode.Create,
                    true
                )
            )
            {
                foreach (var doc in result.Documents ?? [])
                {
                    var entry = archive.CreateEntry(doc.FileName);
                    await using var entryStream = entry.Open();
                    await entryStream.WriteAsync(doc.FileData, 0, doc.FileData.Length);
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return File(memoryStream, "application/zip", $"{result.CollectionName}.zip");
        }

        [HttpPost("Save")]
        public async Task<ActionResult<Outcome<FileCollection>>> SaveCollection(
            [FromBody] FileCollectionSaveInput fileCollection
        )
        {
            var currentUser = await GetCurrentUser();

            var result = await _actionExecutor.ExecuteAsync<
                IFileCollectionProcessingManager,
                FileCollection
            >(service => service.SaveFileCollection(fileCollection, currentUser));

            return new Outcome<FileCollection> { Data = result };
        }

        [HttpPost("GetOneLayer")]
        public async Task<ActionResult<Outcome<FlatFileDocumentPartialCollection>>> GetOneLayer(
            [FromBody] OptionalIdInput input
        )
        {
            var currentUser = await GetCurrentUser();

            var result = await _actionExecutor.ExecuteAsync<
                IFileCollectionProcessingManager,
                FlatFileDocumentPartialCollection
            >(service => service.GetOneLayerFileDocPartialsAndCollections(currentUser, input.Id));

            return new Outcome<FlatFileDocumentPartialCollection> { Data = result };
        }

        [HttpPost("Delete")]
        public async Task<ActionResult<Outcome<Guid>>> Delete([FromBody] RequiredGuidIdInput input)
        {
            var currentUser = await GetCurrentUser();
            
            var result = await _actionExecutor.ExecuteAsync<IFileCollectionProcessingManager, Guid>(
                service => service.DeleteFileCollection(input.Id, currentUser)
            );

            return new Outcome<Guid> { Data = result };
        }
    }
}
