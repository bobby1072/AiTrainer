using System.Text.Json;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Helpers;
using AiTrainer.Web.Domain.Models.Partials;
using AiTrainer.Web.Domain.Services.File.Concrete;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.TestBase;
using AiTrainer.Web.TestBase.Utils;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AiTrainer.Web.Domain.Services.Tests;

public sealed class FileCollectionFaissRemoveDocumentsProcessingManagerTests : AiTrainerTestBase
{
    private readonly Mock<ICoreClient<CoreRemoveDocumentsFromStoreInput, CoreFaissStoreResponse>> _mockCoreClient = new();
    private readonly Mock<IFileCollectionFaissRepository> _mockFileCollectionFaissRepo = new();
    private readonly Mock<IFileDocumentRepository> _mockFileDocumentRepo = new();
    private readonly ILogger<FileCollectionFaissRemoveDocumentsProcessingManager> _logger = 
        new NullLogger<FileCollectionFaissRemoveDocumentsProcessingManager>();

    private FileCollectionFaissRemoveDocumentsProcessingManager CreateSut()
    {
        SetUpBasicHttpContext(addCorrelationId: true);
        
        return new FileCollectionFaissRemoveDocumentsProcessingManager(
            _mockFileCollectionFaissRepo.Object,
            _mockFileDocumentRepo.Object,
            _logger,
            _mockCoreClient.Object,
            _mockHttpContextAccessor.Object
        );
    }

    [Fact]
    public async Task RemoveDocumentsFromFaissStoreSafelyAsync_WhenNoDocumentsToRemove_ReturnsOriginalFaissStore()
    {
        // Arrange
        var sut = CreateSut();
        var testFaissStore = await TestFaissUtils.GetTestFaissStoreAsync();
        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, testFaissStore.DocStore)
            .With(x => x.FaissIndex, testFaissStore.FaissIndex)
            .Create();
        
        // Get all existing document IDs from the FAISS store so nothing should be removed
        var existingDocumentIds = fileCollectionFaiss.SingleDocuments.Value
            .Select(x => x.FileDocumentId)
            .ToArray();

        // Act
        var result = await sut.RemoveDocumentsFromFaissStoreSafelyAsync(
            fileCollectionFaiss, 
            existingDocumentIds, 
            CancellationToken.None);

        // Assert
        Assert.Equal(fileCollectionFaiss, result);
        _mockCoreClient.Verify(x => x.TryInvokeAsync(It.IsAny<CoreRemoveDocumentsFromStoreInput>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RemoveDocumentsFromFaissStoreSafelyAsync_WhenDocumentsToRemove_CallsCoreClientAndReturnsUpdatedStore()
    {
        // Arrange
        var sut = CreateSut();
        var testFaissStore = await TestFaissUtils.GetTestFaissStoreAsync();
        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, testFaissStore.DocStore)
            .With(x => x.FaissIndex, testFaissStore.FaissIndex)
            .Create();

        // Get actual FileDocumentIds from the FAISS store 
        var allDocumentIds = fileCollectionFaiss.SingleDocuments.Value
            .Select(x => x.FileDocumentId)
            .Distinct()
            .ToArray();
        
        // Pass an empty array to exclude NO documents (meaning all documents should be removed)
        // Or pass IDs that DON'T exist in the store to force removal of all documents
        var documentsToKeep = new Guid[0]; // Empty array means keep nothing = remove everything
        
        // Ensure we actually have documents to remove
        Assert.True(fileCollectionFaiss.SingleDocuments.Value.Count > 0, "Test setup should have documents in FAISS store");
        
        var expectedCoreResponse = _fixture.Build<CoreFaissStoreResponse>()
            .With(x => x.JsonDocStore, JsonDocument.Parse("{}"))
            .Create();
        _mockCoreClient.Setup(x => x.TryInvokeAsync(It.IsAny<CoreRemoveDocumentsFromStoreInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCoreResponse);

        // Act
        var result = await sut.RemoveDocumentsFromFaissStoreSafelyAsync(
            fileCollectionFaiss, 
            documentsToKeep, 
            CancellationToken.None);

        // Assert
        Assert.NotSame(fileCollectionFaiss, result); // Verify different object instances
        Assert.Equal(fileCollectionFaiss.Id, result.Id);
        Assert.Equal(fileCollectionFaiss.UserId, result.UserId);
        Assert.Equal(fileCollectionFaiss.CollectionId, result.CollectionId);
        Assert.Equal(expectedCoreResponse.IndexFile, result.FaissIndex);
        Assert.Equal(expectedCoreResponse.JsonDocStore, result.FaissJson);
        Assert.Equal(fileCollectionFaiss.DateCreated, result.DateCreated);
        Assert.Equal(fileCollectionFaiss.DateModified, result.DateModified);

        _mockCoreClient.Verify(x => x.TryInvokeAsync(
            It.Is<CoreRemoveDocumentsFromStoreInput>(input => 
                input.FileInput == fileCollectionFaiss.FaissIndex &&
                input.DocStore == fileCollectionFaiss.FaissJson &&
                input.DocumentIdsToRemove.Count > 0), // Just verify some documents are being removed
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task RemoveDocumentsFromFaissStoreSafelyAsync_WhenCoreClientReturnsNull_ThrowsApiException()
    {
        // Arrange
        var sut = CreateSut();
        var testFaissStore = await TestFaissUtils.GetTestFaissStoreAsync();
        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, testFaissStore.DocStore)
            .With(x => x.FaissIndex, testFaissStore.FaissIndex)
            .Create();

        var existingDocumentIds = new Guid[0]; // No existing documents, so all should be removed
        
        _mockCoreClient.Setup(x => x.TryInvokeAsync(It.IsAny<CoreRemoveDocumentsFromStoreInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CoreFaissStoreResponse?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => 
            sut.RemoveDocumentsFromFaissStoreSafelyAsync(fileCollectionFaiss, existingDocumentIds, CancellationToken.None));
        
        Assert.Equal("Failed to delete chunks from the chosen faiss store", exception.Message);
    }

    [Fact]
    public async Task RemoveDocumentsFromFaissStoreAndUpdateItAsync_WhenFaissStoreNotFound_ReturnsWithoutAction()
    {
        // Arrange
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var collectionId = Guid.NewGuid();
        var currentUser = _fixture.Build<Domain.Models.User>().With(x => x.Id, userId).Create();

        _mockFileCollectionFaissRepo.Setup(x => x.ByUserAndCollectionId(userId, collectionId))
            .ReturnsAsync(new DbGetOneResult<FileCollectionFaiss>(null));

        // Act
        await sut.RemoveDocumentsFromFaissStoreAndUpdateItAsync(collectionId, currentUser, CancellationToken.None);

        // Assert
        _mockFileDocumentRepo.Verify(x => x.GetManyDocumentPartialsByCollectionIdAndUserId(It.IsAny<Guid>(), It.IsAny<Guid?>()), Times.Never);
        _mockCoreClient.Verify(x => x.TryInvokeAsync(It.IsAny<CoreRemoveDocumentsFromStoreInput>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RemoveDocumentsFromFaissStoreAndUpdateItAsync_WhenFailsToRetrieveFaissStore_ThrowsApiException()
    {
        // Arrange
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var collectionId = Guid.NewGuid();
        var currentUser = _fixture.Build<Domain.Models.User>().With(x => x.Id, userId).Create();

        _mockFileCollectionFaissRepo.Setup(x => x.ByUserAndCollectionId(userId, collectionId))
            .ReturnsAsync((DbGetOneResult<FileCollectionFaiss>?)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => 
            sut.RemoveDocumentsFromFaissStoreAndUpdateItAsync(collectionId, currentUser, CancellationToken.None));
        
        Assert.Equal("Failed to retrieve file collection faiss store", exception.Message);
    }

    [Fact]
    public async Task RemoveDocumentsFromFaissStoreAndUpdateItAsync_WhenNoDocumentsToRemove_ReturnsWithoutCoreClientCall()
    {
        // Arrange
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var collectionId = Guid.NewGuid();
        var currentUser = _fixture.Build<Domain.Models.User>().With(x => x.Id, userId).Create();
        
        var testFaissStore = await TestFaissUtils.GetTestFaissStoreAsync();
        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, testFaissStore.DocStore)
            .With(x => x.FaissIndex, testFaissStore.FaissIndex)
            .With(x => x.UserId, userId)
            .With(x => x.CollectionId, collectionId)
            .Create();

        _mockFileCollectionFaissRepo.Setup(x => x.ByUserAndCollectionId(userId, collectionId))
            .ReturnsAsync(new DbGetOneResult<FileCollectionFaiss>(fileCollectionFaiss));

        // Setup existing documents to match all documents in FAISS store
        var allDocumentIds = fileCollectionFaiss.SingleDocuments.Value.Select(x => x.FileDocumentId).ToArray();
        var existingDocuments = allDocumentIds.Select(id => _fixture.Build<FileDocumentPartial>().With(x => x.Id, id).Create()).ToArray();
        
        _mockFileDocumentRepo.Setup(x => x.GetManyDocumentPartialsByCollectionIdAndUserId(userId, collectionId))
            .ReturnsAsync(new DbGetManyResult<FileDocumentPartial>(existingDocuments));

        // Act
        await sut.RemoveDocumentsFromFaissStoreAndUpdateItAsync(collectionId, currentUser, CancellationToken.None);

        // Assert
        _mockCoreClient.Verify(x => x.TryInvokeAsync(It.IsAny<CoreRemoveDocumentsFromStoreInput>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockFileCollectionFaissRepo.Verify(x => x.Delete(It.IsAny<IReadOnlyCollection<long>>()), Times.Never);
        _mockFileCollectionFaissRepo.Verify(x => x.Update(It.IsAny<IReadOnlyCollection<FileCollectionFaiss>>()), Times.Never);
    }

    [Fact]
    public async Task RemoveDocumentsFromFaissStoreAndUpdateItAsync_WhenDocumentsToRemove_CallsRemoveDirectlyFromStoreAndSave()
    {
        // Arrange
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var collectionId = Guid.NewGuid();
        var currentUser = _fixture.Build<Domain.Models.User>().With(x => x.Id, userId).Create();
        
        var testFaissStore = await TestFaissUtils.GetTestFaissStoreAsync();
        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, testFaissStore.DocStore)
            .With(x => x.FaissIndex, testFaissStore.FaissIndex)
            .With(x => x.UserId, userId)
            .With(x => x.CollectionId, collectionId)
            .Create();

        _mockFileCollectionFaissRepo.Setup(x => x.ByUserAndCollectionId(userId, collectionId))
            .ReturnsAsync(new DbGetOneResult<FileCollectionFaiss>(fileCollectionFaiss));

        // Setup existing documents to exclude some documents from FAISS store
        // To force documents to be removed, we need to return existing document IDs that DON'T match the FAISS store
        // Since all documents in the test store have the same FileDocumentId, we'll return different IDs to force removal
        var existingDocumentIds = new[] { Guid.NewGuid(), Guid.NewGuid() }; // IDs that don't exist in the FAISS store
        var existingDocuments = existingDocumentIds.Select(id => _fixture.Build<FileDocumentPartial>().With(x => x.Id, id).Create()).ToArray();
        
        _mockFileDocumentRepo.Setup(x => x.GetManyDocumentPartialsByCollectionIdAndUserId(userId, collectionId))
            .ReturnsAsync(new DbGetManyResult<FileDocumentPartial>(existingDocuments));

        var coreResponse = _fixture.Build<CoreFaissStoreResponse>()
            .With(x => x.JsonDocStore, JsonDocument.Parse("""
                [
                    [
                        [
                            "4f53c746-e137-4634-9603-7afdea91f223",
                            {
                                "pageContent": "This document remains after removal",
                                "metadata": {
                                    "Id": "4f53c746-e137-4634-9603-7afdea91f223",
                                    "FileDocumentId": "0fd5c36c-23bd-466c-87ca-c04ff7cd56b6"
                                }
                            }
                        ]
                    ],
                    {
                        "0": "4f53c746-e137-4634-9603-7afdea91f223"
                    }
                ]
                """))
            .Create();
        _mockCoreClient.Setup(x => x.TryInvokeAsync(It.IsAny<CoreRemoveDocumentsFromStoreInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(coreResponse);

        // Create a new FAISS store with remaining documents for the update operation
        var updatedFaissStore = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, coreResponse.JsonDocStore)
            .With(x => x.FaissIndex, coreResponse.IndexFile)
            .Create();

        _mockFileCollectionFaissRepo.Setup(x => x.Update(It.IsAny<IReadOnlyCollection<FileCollectionFaiss>>()))
            .ReturnsAsync(new DbSaveResult<FileCollectionFaiss>([updatedFaissStore]) { IsSuccessful = true });

        // Act
        await sut.RemoveDocumentsFromFaissStoreAndUpdateItAsync(collectionId, currentUser, CancellationToken.None);

        // Assert
        _mockCoreClient.Verify(x => x.TryInvokeAsync(
            It.Is<CoreRemoveDocumentsFromStoreInput>(input => 
                input.FileInput == fileCollectionFaiss.FaissIndex &&
                input.DocStore == fileCollectionFaiss.FaissJson &&
                input.DocumentIdsToRemove.Count == 5), // All 5 documents in test store should be removed
            It.IsAny<CancellationToken>()), Times.Once);

        _mockFileCollectionFaissRepo.Verify(x => x.Update(
            It.Is<IReadOnlyCollection<FileCollectionFaiss>>(stores => 
                stores.Count == 1 &&
                stores.First().Id == fileCollectionFaiss.Id &&
                stores.First().UserId == userId &&
                stores.First().CollectionId == collectionId &&
                stores.First().FaissIndex == coreResponse.IndexFile &&
                stores.First().FaissJson == coreResponse.JsonDocStore)),
            Times.Once);
    }

    [Fact]
    public async Task RemoveDocumentsFromFaissStoreAndUpdateItAsync_WhenFailsToRetrieveFileDocuments_ThrowsApiException()
    {
        // Arrange
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var collectionId = Guid.NewGuid();
        var currentUser = _fixture.Build<Domain.Models.User>().With(x => x.Id, userId).Create();
        
        var testFaissStore = await TestFaissUtils.GetTestFaissStoreAsync();
        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, testFaissStore.DocStore)
            .With(x => x.FaissIndex, testFaissStore.FaissIndex)
            .Create();

        _mockFileCollectionFaissRepo.Setup(x => x.ByUserAndCollectionId(userId, collectionId))
            .ReturnsAsync(new DbGetOneResult<FileCollectionFaiss>(fileCollectionFaiss));

        _mockFileDocumentRepo.Setup(x => x.GetManyDocumentPartialsByCollectionIdAndUserId(userId, collectionId))
            .ReturnsAsync((DbGetManyResult<FileDocumentPartial>?)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => 
            sut.RemoveDocumentsFromFaissStoreAndUpdateItAsync(collectionId, currentUser, CancellationToken.None));
        
        Assert.Equal("Failed to retrieve file documents", exception.Message);
    }

    [Fact]
    public async Task RemoveDocumentsFromFaissStoreAndUpdateItAsync_WhenNoDocumentsLeftAfterRemoval_DeletesFaissStore()
    {
        // Arrange
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var collectionId = Guid.NewGuid();
        var currentUser = _fixture.Build<Domain.Models.User>().With(x => x.Id, userId).Create();
        
        var testFaissStore = await TestFaissUtils.GetTestFaissStoreAsync();
        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, testFaissStore.DocStore)
            .With(x => x.FaissIndex, testFaissStore.FaissIndex)
            .With(x => x.UserId, userId)
            .With(x => x.CollectionId, collectionId)
            .Create();

        _mockFileCollectionFaissRepo.Setup(x => x.ByUserAndCollectionId(userId, collectionId))
            .ReturnsAsync(new DbGetOneResult<FileCollectionFaiss>(fileCollectionFaiss));

        // No existing documents, so all should be removed
        _mockFileDocumentRepo.Setup(x => x.GetManyDocumentPartialsByCollectionIdAndUserId(userId, collectionId))
            .ReturnsAsync(new DbGetManyResult<FileDocumentPartial>(new FileDocumentPartial[0]));

        // Mock core client to return empty FAISS store
        var emptyDocStore = JsonDocument.Parse("[[]]");
        var coreResponse = _fixture.Build<CoreFaissStoreResponse>()
            .With(x => x.JsonDocStore, emptyDocStore)
            .Create();
        
        _mockCoreClient.Setup(x => x.TryInvokeAsync(It.IsAny<CoreRemoveDocumentsFromStoreInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(coreResponse);

        _mockFileCollectionFaissRepo.Setup(x => x.Delete(It.IsAny<IReadOnlyCollection<long>>()))
            .ReturnsAsync(new DbDeleteResult<long>([(long)fileCollectionFaiss.Id!]));

        // Act
        await sut.RemoveDocumentsFromFaissStoreAndUpdateItAsync(collectionId, currentUser, CancellationToken.None);

        // Assert
        _mockFileCollectionFaissRepo.Verify(x => x.Delete(
            It.Is<IReadOnlyCollection<long>>(ids => 
                ids.Count == 1 && ids.First() == (long)fileCollectionFaiss.Id!)),
            Times.Once);
        
        _mockFileCollectionFaissRepo.Verify(x => x.Update(It.IsAny<IReadOnlyCollection<FileCollectionFaiss>>()), Times.Never);
    }

    [Fact]
    public async Task RemoveDocumentsFromFaissStoreAndUpdateItAsync_WhenCoreClientFails_ThrowsApiException()
    {
        // Arrange
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var collectionId = Guid.NewGuid();
        var currentUser = _fixture.Build<Domain.Models.User>().With(x => x.Id, userId).Create();
        
        var testFaissStore = await TestFaissUtils.GetTestFaissStoreAsync();
        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, testFaissStore.DocStore)
            .With(x => x.FaissIndex, testFaissStore.FaissIndex)
            .Create();

        _mockFileCollectionFaissRepo.Setup(x => x.ByUserAndCollectionId(userId, collectionId))
            .ReturnsAsync(new DbGetOneResult<FileCollectionFaiss>(fileCollectionFaiss));

        _mockFileDocumentRepo.Setup(x => x.GetManyDocumentPartialsByCollectionIdAndUserId(userId, collectionId))
            .ReturnsAsync(new DbGetManyResult<FileDocumentPartial>(new FileDocumentPartial[0]));

        _mockCoreClient.Setup(x => x.TryInvokeAsync(It.IsAny<CoreRemoveDocumentsFromStoreInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CoreFaissStoreResponse?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => 
            sut.RemoveDocumentsFromFaissStoreAndUpdateItAsync(collectionId, currentUser, CancellationToken.None));
        
        Assert.Equal("Failed to delete chunks from the chosen faiss store", exception.Message);
    }

    [Fact]
    public async Task RemoveDocumentsFromFaissStoreAndUpdateItAsync_WhenDeleteFaissStoreFails_ThrowsApiException()
    {
        // Arrange
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var collectionId = Guid.NewGuid();
        var currentUser = _fixture.Build<Domain.Models.User>().With(x => x.Id, userId).Create();
        
        var testFaissStore = await TestFaissUtils.GetTestFaissStoreAsync();
        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, testFaissStore.DocStore)
            .With(x => x.FaissIndex, testFaissStore.FaissIndex)
            .Create();

        _mockFileCollectionFaissRepo.Setup(x => x.ByUserAndCollectionId(userId, collectionId))
            .ReturnsAsync(new DbGetOneResult<FileCollectionFaiss>(fileCollectionFaiss));

        _mockFileDocumentRepo.Setup(x => x.GetManyDocumentPartialsByCollectionIdAndUserId(userId, collectionId))
            .ReturnsAsync(new DbGetManyResult<FileDocumentPartial>(new FileDocumentPartial[0]));

        // Mock core client to return empty FAISS store
        var emptyDocStore = JsonDocument.Parse("[[]]");
        var coreResponse = _fixture.Build<CoreFaissStoreResponse>()
            .With(x => x.JsonDocStore, emptyDocStore)
            .Create();
        
        _mockCoreClient.Setup(x => x.TryInvokeAsync(It.IsAny<CoreRemoveDocumentsFromStoreInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(coreResponse);

        _mockFileCollectionFaissRepo.Setup(x => x.Delete(It.IsAny<IReadOnlyCollection<long>>()))
            .ReturnsAsync(new DbDeleteResult<long>(null!) { Data = null! });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => 
            sut.RemoveDocumentsFromFaissStoreAndUpdateItAsync(collectionId, currentUser, CancellationToken.None));
        
        Assert.Equal("Failed to delete faiss store", exception.Message);
    }

    [Fact]
    public async Task RemoveDocumentsFromFaissStoreAndUpdateItAsync_WhenUpdateFaissStoreFails_ThrowsApiException()
    {
        // Arrange
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var collectionId = Guid.NewGuid();
        var currentUser = _fixture.Build<Domain.Models.User>().With(x => x.Id, userId).Create();
        
        var testFaissStore = await TestFaissUtils.GetTestFaissStoreAsync();
        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, testFaissStore.DocStore)
            .With(x => x.FaissIndex, testFaissStore.FaissIndex)
            .Create();

        _mockFileCollectionFaissRepo.Setup(x => x.ByUserAndCollectionId(userId, collectionId))
            .ReturnsAsync(new DbGetOneResult<FileCollectionFaiss>(fileCollectionFaiss));

        // Setup existing documents to exclude some documents from FAISS store
        // To force documents to be removed, we need to return existing document IDs that DON'T match the FAISS store
        // Since all documents in the test store have the same FileDocumentId, we'll return different IDs to force removal
        var existingDocumentIds = new[] { Guid.NewGuid(), Guid.NewGuid() }; // IDs that don't exist in the FAISS store
        var existingDocuments = existingDocumentIds.Select(id => _fixture.Build<FileDocumentPartial>().With(x => x.Id, id).Create()).ToArray();
        
        _mockFileDocumentRepo.Setup(x => x.GetManyDocumentPartialsByCollectionIdAndUserId(userId, collectionId))
            .ReturnsAsync(new DbGetManyResult<FileDocumentPartial>(existingDocuments));

        var coreResponse = _fixture.Build<CoreFaissStoreResponse>()
            .With(x => x.JsonDocStore, JsonDocument.Parse("""
                [
                    [
                        [
                            "4f53c746-e137-4634-9603-7afdea91f223",
                            {
                                "pageContent": "This document remains after removal",
                                "metadata": {
                                    "Id": "4f53c746-e137-4634-9603-7afdea91f223",
                                    "FileDocumentId": "0fd5c36c-23bd-466c-87ca-c04ff7cd56b6"
                                }
                            }
                        ]
                    ],
                    {
                        "0": "4f53c746-e137-4634-9603-7afdea91f223"
                    }
                ]
                """))
            .Create();
        _mockCoreClient.Setup(x => x.TryInvokeAsync(It.IsAny<CoreRemoveDocumentsFromStoreInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(coreResponse);

        _mockFileCollectionFaissRepo.Setup(x => x.Update(It.IsAny<IReadOnlyCollection<FileCollectionFaiss>>()))
            .ReturnsAsync(new DbSaveResult<FileCollectionFaiss>(null) { IsSuccessful = false });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => 
            sut.RemoveDocumentsFromFaissStoreAndUpdateItAsync(collectionId, currentUser, CancellationToken.None));
        
        Assert.Equal("Failed to save new updated faiss store", exception.Message);
    }

    [Fact]
    public async Task RemoveDocumentsFromFaissStoreAndUpdateItAsync_WithValidScenario_UpdatesDateModified()
    {
        // Arrange
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var collectionId = Guid.NewGuid();
        var currentUser = _fixture.Build<Domain.Models.User>().With(x => x.Id, userId).Create();
        
        var testFaissStore = await TestFaissUtils.GetTestFaissStoreAsync();
        var originalDateModified = DateTime.UtcNow.AddDays(-1);
        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, testFaissStore.DocStore)
            .With(x => x.FaissIndex, testFaissStore.FaissIndex)
            .With(x => x.UserId, userId)
            .With(x => x.CollectionId, collectionId)
            .With(x => x.DateModified, originalDateModified)
            .Create();

        _mockFileCollectionFaissRepo.Setup(x => x.ByUserAndCollectionId(userId, collectionId))
            .ReturnsAsync(new DbGetOneResult<FileCollectionFaiss>(fileCollectionFaiss));

        // Setup existing documents to exclude some documents from FAISS store
        // To force documents to be removed, we need to return existing document IDs that DON'T match the FAISS store
        // Since all documents in the test store have the same FileDocumentId, we'll return different IDs to force removal
        var existingDocumentIds = new[] { Guid.NewGuid(), Guid.NewGuid() }; // IDs that don't exist in the FAISS store
        var existingDocuments = existingDocumentIds.Select(id => _fixture.Build<FileDocumentPartial>().With(x => x.Id, id).Create()).ToArray();
        
        _mockFileDocumentRepo.Setup(x => x.GetManyDocumentPartialsByCollectionIdAndUserId(userId, collectionId))
            .ReturnsAsync(new DbGetManyResult<FileDocumentPartial>(existingDocuments));

        var coreResponse = _fixture.Build<CoreFaissStoreResponse>()
            .With(x => x.JsonDocStore, JsonDocument.Parse("""
                [
                    [
                        [
                            "4f53c746-e137-4634-9603-7afdea91f223",
                            {
                                "pageContent": "This document remains after removal",
                                "metadata": {
                                    "Id": "4f53c746-e137-4634-9603-7afdea91f223",
                                    "FileDocumentId": "0fd5c36c-23bd-466c-87ca-c04ff7cd56b6"
                                }
                            }
                        ]
                    ],
                    {
                        "0": "4f53c746-e137-4634-9603-7afdea91f223"
                    }
                ]
                """))
            .Create();
        _mockCoreClient.Setup(x => x.TryInvokeAsync(It.IsAny<CoreRemoveDocumentsFromStoreInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(coreResponse);

        var updatedFaissStore = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, coreResponse.JsonDocStore)
            .Create();
        _mockFileCollectionFaissRepo.Setup(x => x.Update(It.IsAny<IReadOnlyCollection<FileCollectionFaiss>>()))
            .ReturnsAsync(new DbSaveResult<FileCollectionFaiss>([updatedFaissStore]) { IsSuccessful = true });

        // Act
        await sut.RemoveDocumentsFromFaissStoreAndUpdateItAsync(collectionId, currentUser, CancellationToken.None);

        // Assert
        _mockFileCollectionFaissRepo.Verify(x => x.Update(
            It.Is<IReadOnlyCollection<FileCollectionFaiss>>(stores => 
                stores.Count == 1 &&
                stores.First().DateModified > originalDateModified &&
                stores.First().DateCreated == fileCollectionFaiss.DateCreated)),
            Times.Once);
    }
}
