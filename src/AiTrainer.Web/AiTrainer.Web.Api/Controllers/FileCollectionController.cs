using System.IO.Compression;
using System.Net;
using System.Text.Json;
using AiTrainer.Web.Api.Attributes;
using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Views;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.File.Abstract;
using BT.Common.FastArray.Proto;
using Microsoft.AspNetCore.Mvc;

namespace AiTrainer.Web.Api.Controllers
{
    [RequireUserLogin]
    public sealed class FileCollectionController : BaseController
    {
        public FileCollectionController(IHttpDomainServiceActionExecutor actionExecutor)
            : base(actionExecutor) { }

        [HttpPost("UnshareWithMember")]
        public async Task<ActionResult<Outcome<Guid>>> Share(
            [FromBody] RequiredGuidIdInput fileCollectionMemberSaveInput)
        {
            var currentUser = await GetCurrentUser();
            var result =
                await _actionExecutor
                    .ExecuteAsync<IFileCollectionProcessingManager, Guid>(
                        serv => serv.UnshareFileCollectionAsync(fileCollectionMemberSaveInput, currentUser));

            return new Outcome<Guid>
            {
                Data = result
            };
        }
        [HttpPost("ShareWithMembers")]
        public async Task<ActionResult<Outcome<IReadOnlyCollection<SharedFileCollectionMember>>>> Share(
            [FromBody] JsonDocument fileCollectionMemberSaveInput)
        {
            var currentUser = await GetCurrentUser();
            var deserializedInput = DeserializeSharedFileCollectionMemberSaveInput(fileCollectionMemberSaveInput);

            var result = await _actionExecutor
                .ExecuteAsync<IFileCollectionProcessingManager, IReadOnlyCollection<SharedFileCollectionMember>>
                    (serv => serv.ShareFileCollectionAsync(deserializedInput, currentUser), nameof(IFileCollectionProcessingManager.ShareFileCollectionAsync));
            
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
        
        
        private static SharedFileCollectionMemberSaveInput DeserializeSharedFileCollectionMemberSaveInput(JsonDocument genericMemberSaveInput)
        {
            Guid collectionId;
            var singleMemberInput = new List<SharedFileCollectionSingleMemberSaveInput>();
            try
            {
                var rawCollectionId = genericMemberSaveInput.RootElement.GetProperty(
                    SharedFileCollectionMemberSaveInput.CollectionIdJsonName).GetString() ?? throw new JsonException("Failed to get collection id from input");
                collectionId = Guid.Parse(rawCollectionId);
                var enumeratedMembers = genericMemberSaveInput.RootElement
                    .GetProperty(SharedFileCollectionMemberSaveInput.MembersToShareToJsonName).EnumerateArray();

                foreach (var member in enumeratedMembers)
                {
                    if (!string.IsNullOrEmpty(member
                            .GetProperty(SharedFileCollectionSingleMemberEmailSaveInput.EmailJsonName)
                            .GetString()))
                    {
                        singleMemberInput.Add(
                            JsonSerializer
                                .Deserialize<SharedFileCollectionSingleMemberEmailSaveInput>(member.GetRawText()) ?? throw new JsonException($"Failed to deserialize to {nameof(SharedFileCollectionSingleMemberEmailSaveInput)}")    
                        );
                    }
                    else if (!string.IsNullOrEmpty(member
                            .GetProperty(SharedFileCollectionSingleMemberUserIdSaveInput.UserIdJsonName)
                            .GetString()))
                    {
                        singleMemberInput.Add(
                            JsonSerializer
                                .Deserialize<SharedFileCollectionSingleMemberUserIdSaveInput>(member.GetRawText()) ?? throw new JsonException($"Failed to deserialize to {nameof(SharedFileCollectionSingleMemberEmailSaveInput)}")    
                        );
                    }   
                }
            }
            catch
            {
                throw new ApiException("Failed to deserialize json input", HttpStatusCode.BadGateway);
            }


            return new SharedFileCollectionMemberSaveInput
            {
                CollectionId = collectionId,
                MembersToShareTo = singleMemberInput.ToArray(),
            };
        } 
    }
}
