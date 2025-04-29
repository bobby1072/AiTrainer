using System.IO.Compression;
using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Views;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.File.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    [RequireUserLogin]
    public class FileCollectionController : BaseController
    {
        public FileCollectionController(IHttpDomainServiceActionExecutor actionExecutor)
            : base(actionExecutor) { }

        [HttpPost("Download")]
        public async Task<IActionResult> Download([FromBody] RequiredGuidIdInput input)
        {
            var currentUser = await GetCurrentUser();
            var result = await _actionExecutor.ExecuteAsync<
                IFileCollectionProcessingManager,
                FileCollection
            >(service => service.GetFileCollectionWithContents(input.Id, currentUser), nameof(IFileCollectionProcessingManager.GetFileCollectionWithContents));

            await using var memoryStream = new MemoryStream();
            using (
                var archive = new ZipArchive(
                    memoryStream,
                    ZipArchiveMode.Create,
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
            >(service => service.SaveFileCollection(fileCollection, currentUser), nameof(IFileCollectionProcessingManager.SaveFileCollection));

            return new Outcome<FileCollection> { Data = result };
        }

        [HttpPost("GetOneLayer")]
        public async Task<ActionResult<Outcome<FlatFileDocumentPartialCollectionView>>> GetOneLayer(
            [FromBody] OptionalIdInput input
        )
        {
            var currentUser = await GetCurrentUser();

            var result = await _actionExecutor.ExecuteAsync<
                IFileCollectionProcessingManager,
                FlatFileDocumentPartialCollectionView
            >(service => service.GetOneLayerFileDocPartialsAndCollections(currentUser, input.Id), nameof(IFileCollectionProcessingManager.GetOneLayerFileDocPartialsAndCollections));

            return new Outcome<FlatFileDocumentPartialCollectionView> { Data = result };
        }

        [HttpPost("Delete")]
        public async Task<ActionResult<Outcome<Guid>>> Delete([FromBody] RequiredGuidIdInput input)
        {
            var currentUser = await GetCurrentUser();
            
            var result = await _actionExecutor.ExecuteAsync<IFileCollectionProcessingManager, Guid>(
                service => service.DeleteFileCollection(input.Id, currentUser), nameof(IFileCollectionProcessingManager.DeleteFileCollection)
            );

            return new Outcome<Guid> { Data = result };
        }
    }
}
