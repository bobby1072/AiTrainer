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
    public sealed class FileCollectionController : BaseController
    {
        public FileCollectionController(IHttpDomainServiceActionExecutor actionExecutor)
            : base(actionExecutor) { }

        [HttpPost("ShareWithMembers")]
        public async Task<ActionResult<Outcome<IReadOnlyCollection<SharedFileCollectionMember>>>> Share(
            [FromBody] SharedFileCollectionMemberSaveInput fileCollectionMemberSaveInput)
        {
            var currentUser = await GetCurrentUser();
            var result =
                await _actionExecutor
                    .ExecuteAsync<IFileCollectionProcessingManager, IReadOnlyCollection<SharedFileCollectionMember>>(
                        serv => serv.ShareFileCollectionAsync(fileCollectionMemberSaveInput, currentUser));

            return new Outcome<IReadOnlyCollection<SharedFileCollectionMember>>
            {
                Data = result
            };
        }
        [HttpPost("Download")]
        public async Task<IActionResult> Download([FromBody] RequiredGuidIdInput input)
        {
            var currentUser = await GetCurrentUser();
            var result = await _actionExecutor.ExecuteAsync<
                IFileCollectionProcessingManager,
                FileCollection
            >(service => service.GetFileCollectionWithContentsAsync(input.Id, currentUser), nameof(IFileCollectionProcessingManager.GetFileCollectionWithContentsAsync));

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
            >(service => service.SaveFileCollectionAsync(fileCollection, currentUser), nameof(IFileCollectionProcessingManager.SaveFileCollectionAsync));

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
            >(service => service.GetOneLayerFileDocPartialsAndCollectionsAsync(currentUser, input.Id), nameof(IFileCollectionProcessingManager.GetOneLayerFileDocPartialsAndCollectionsAsync));

            return new Outcome<FlatFileDocumentPartialCollectionView> { Data = result };
        }

        [HttpPost("Delete")]
        public async Task<ActionResult<Outcome<Guid>>> Delete([FromBody] RequiredGuidIdInput input)
        {
            var currentUser = await GetCurrentUser();
            
            var result = await _actionExecutor.ExecuteAsync<IFileCollectionProcessingManager, Guid>(
                service => service.DeleteFileCollectionAsync(input.Id, currentUser), nameof(IFileCollectionProcessingManager.DeleteFileCollectionAsync)
            );

            return new Outcome<Guid> { Data = result };
        }
    }
}
