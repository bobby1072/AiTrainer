using System.Text;
using System.Text.Json;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Services.File.Concrete;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Repositories.Concrete;
using AiTrainer.Web.TestBase;
using AiTrainer.Web.TestBase.Utils;
using AutoFixture;
using BT.Common.OperationTimer.Common;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiTrainer.Web.Domain.Services.Tests;

public class FileCollectionFaissSyncProcessingManagerTests: AiTrainerTestBase
{
    private readonly Mock<ICoreClient<
        CoreDocumentToChunkInput,
        CoreChunkedDocumentResponse
    >> _mockDocumentChunkerClient = new();
    private readonly Mock<ICoreClient<
        CoreCreateFaissStoreInput,
        CoreFaissStoreResponse
    >> _mockCreateFaissStoreService = new();
    private readonly Mock<ICoreClient<
        CoreUpdateFaissStoreInput,
        CoreFaissStoreResponse
    >> _mockUpdateFaissStoreService = new();
    private readonly Mock<IFileCollectionFaissRepository> _mockFileCollectionFaissRepository = new();
    private readonly Mock<IFileDocumentRepository> _mockFileDocumentRepository = new();

    private readonly int _syncRetryAmount = new Random().Next(5, 50);
    private FaissSyncRetrySettingsConfiguration _retrySettings => new()
    {
        UseRetry = true,
        TotalAttempts = _syncRetryAmount,
        TimeoutInSeconds = 15,
        DelayBetweenAttemptsInSeconds = 0
    };
    private readonly FileCollectionFaissSyncProcessingManager _faissSyncProcessingManager;

    public FileCollectionFaissSyncProcessingManagerTests()
    {
        _faissSyncProcessingManager = new FileCollectionFaissSyncProcessingManager(
            _mockDocumentChunkerClient.Object,
            _mockCreateFaissStoreService.Object,
            _mockUpdateFaissStoreService.Object,
            Mock.Of<ILogger<FileCollectionFaissSyncProcessingManager>>(),
            _mockFileDocumentRepository.Object,
            _mockFileCollectionFaissRepository.Object,
            new TestOptionsSnapshot<FaissSyncRetrySettingsConfiguration>(_retrySettings).Object);
    }
    [Fact]
    public async Task SyncUserFileCollectionFaissStore_Should_Retry_The_Correct_Amount_Based_On_Configuration()
    {
        //Arrange
        var collectionId = Guid.NewGuid();
        var currentUser = _fixture
            .Build<Models.User>()
            .With(x => x.Id, Guid.NewGuid())
            .Create();

        _mockFileDocumentRepository.Setup(x =>
                x.GetDocumentsBySync(false, (Guid)currentUser.Id!, collectionId, nameof(FileDocumentEntity.MetaData)))
                .ThrowsAsync(new Exception());
        
        //Act
        var act = () => _faissSyncProcessingManager.SyncUserFileCollectionFaissStore(currentUser, collectionId);
        
        //Assert
        var timerEx = await Assert.ThrowsAsync<OperationTimerException>(act);
        Assert.IsType<ApiException>(timerEx.InnerException);
        _mockFileDocumentRepository.Verify(x =>
            x.GetDocumentsBySync(false, (Guid)currentUser.Id!, collectionId, nameof(FileDocumentEntity.MetaData)), Times.Exactly(_syncRetryAmount));
    }

    [Fact]
    public async Task SyncUserFileCollectionFaissStore_Should_Exit_GraceFully_If_No_Unsynced_Documents()
    {
        //Arrange
        var collectionId = Guid.NewGuid();
        var currentUser = _fixture
            .Build<Models.User>()
            .With(x => x.Id, Guid.NewGuid())
            .Create();
        
        _mockFileDocumentRepository.Setup(x =>
                x.GetDocumentsBySync(false, (Guid)currentUser.Id!, collectionId, nameof(FileDocumentEntity.MetaData)))
            .ReturnsAsync(new DbGetManyResult<FileDocument>());
        
        //Act
        await _faissSyncProcessingManager.SyncUserFileCollectionFaissStore(currentUser, collectionId);
        
        //Assert
        _mockFileDocumentRepository.Verify(x =>
            x.GetDocumentsBySync(false, (Guid)currentUser.Id!, collectionId, nameof(FileDocumentEntity.MetaData)), Times.Once);
    }
    
    [Fact]
    public async Task SyncUserFileCollectionFaissStore_Should_Attempt_To_Create_And_Store_FaissStore_If_One_Does_Not_Exist()
    {
        //Arrange
        var collectionId = Guid.NewGuid();
        var currentUser = _fixture
            .Build<Models.User>()
            .With(x => x.Id, Guid.NewGuid())
            .Create();
        var fileDocInput = _fixture
            .Build<FileDocument>()
            .With(x => x.CollectionId, collectionId)
            .With(x => x.FileData, await TestFileUtils.CreateFileBytes())
            .With(x => x.FaissSynced, false)
            .With(x => x.FileType, FileTypeEnum.Text)
            .CreateMany()
            .ToArray();
        
        var chunkedDocResp = _fixture
            .Build<CoreChunkedDocumentResponse>()
            .With(x => x.DocumentChunks, _fixture.CreateMany<SingleChunkedDocument>().ToArray())
            .Create();
        
        var stringJson = JsonSerializer.Serialize(chunkedDocResp);
        await using var memStream = new MemoryStream(Encoding.UTF8.GetBytes(stringJson));
        
        var faissStore = _fixture
            .Build<CoreFaissStoreResponse>()
            .With(x => x.JsonDocStore, await JsonDocument.ParseAsync(memStream))
            .Create();
        
        _mockFileDocumentRepository
            .Setup(x =>
                x.GetDocumentsBySync(false, (Guid)currentUser.Id!, collectionId, nameof(FileDocumentEntity.MetaData)))
            .ReturnsAsync(new DbGetManyResult<FileDocument>(fileDocInput));
        _mockFileCollectionFaissRepository
            .Setup(x => x.ByUserAndCollectionId((Guid)currentUser.Id!, collectionId))
            .ReturnsAsync(new DbGetOneResult<FileCollectionFaiss>());
        _mockDocumentChunkerClient
            .Setup(x => x.TryInvokeAsync(It.IsAny<CoreDocumentToChunkInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(chunkedDocResp);
        
        _mockCreateFaissStoreService
            .Setup(x => x.TryInvokeAsync(It.IsAny<CoreCreateFaissStoreInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(faissStore);
        _mockFileCollectionFaissRepository
            .Setup(x => x.SaveStoreAndSyncDocs(It.IsAny<FileCollectionFaiss>(), It.IsAny<IReadOnlyCollection<SingleDocumentChunk>>(),It.IsAny<IReadOnlyCollection<Guid>>(), FileCollectionFaissRepositorySaveMode.Create))
            .ReturnsAsync(new DbResult(true));
        
        //Act
        await _faissSyncProcessingManager.SyncUserFileCollectionFaissStore(currentUser, collectionId);
        
        //Assert
        _mockFileDocumentRepository
            .Verify(x =>
                x.GetDocumentsBySync(false, (Guid)currentUser.Id!, collectionId, nameof(FileDocumentEntity.MetaData)), Times.Once);
        _mockFileCollectionFaissRepository
            .Verify(x => x.ByUserAndCollectionId((Guid)currentUser.Id!, collectionId), Times.Once);
        _mockDocumentChunkerClient
            .Verify(x => x.TryInvokeAsync(It.IsAny<CoreDocumentToChunkInput>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockCreateFaissStoreService
            .Verify(x => x.TryInvokeAsync(It.IsAny<CoreCreateFaissStoreInput>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockFileCollectionFaissRepository
            .Verify(x => x.SaveStoreAndSyncDocs(It.IsAny<FileCollectionFaiss>(), It.IsAny<IReadOnlyCollection<SingleDocumentChunk>>(),It.IsAny<IReadOnlyCollection<Guid>>(),
                FileCollectionFaissRepositorySaveMode.Create), Times.Once);
    }
}