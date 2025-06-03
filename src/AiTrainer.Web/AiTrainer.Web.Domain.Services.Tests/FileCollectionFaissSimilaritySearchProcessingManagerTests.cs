using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Services.File.Concrete;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.TestBase;
using AiTrainer.Web.TestBase.Utils;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiTrainer.Web.Domain.Services.Tests;

public sealed class FileCollectionFaissSimilaritySearchProcessingManagerTests: AiTrainerTestBase
{
    private readonly Mock<ICoreClient<CoreSimilaritySearchInput, CoreSimilaritySearchResponse>> _mockSimSearchClient = new();
    private readonly Mock<ILogger<FileCollectionFaissSimilaritySearchProcessingManager>> _mockLogger = new();
    private readonly Mock<IFileCollectionRepository> _mockFileColRepo = new();
    private readonly Mock<IFileCollectionFaissRepository> _mockFileColFaissRepo = new();
    private readonly Mock<IValidator<SimilaritySearchInput>> _mockValidator = new();
    private readonly FileCollectionFaissSimilaritySearchProcessingManager _service;

    public FileCollectionFaissSimilaritySearchProcessingManagerTests()
    {
        _service = new FileCollectionFaissSimilaritySearchProcessingManager(
            _mockSimSearchClient.Object,
            _mockLogger.Object,
            _mockValidator.Object,
            _mockFileColFaissRepo.Object,
            _mockFileColRepo.Object,
            _mockHttpContextAccessor.Object
        );
    }
    [Fact]
    public async Task SimilaritySearch_With_No_Collection_Id_Should_Throw_If_Not_Authorized_User()
    {
        //Arrange
        var currentUser = _fixture.Build<Domain.Models.User>()
            .With(x => x.Id, Guid.NewGuid())
            .Create();
        var testFaissStuff = await TestFaissUtils.GetTestFaissStoreAsync();

        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, testFaissStuff.DocStore)
            .With(x => x.FaissIndex, testFaissStuff.FaissIndex)
            .With(x => x.UserId, Guid.NewGuid())
            .Create();

        var simSearchInput = _fixture
            .Build<SimilaritySearchInput>()
            .With(x => x.CollectionId, (Guid?)null)
            .Create();
        _mockValidator.Setup(v => v.ValidateAsync(simSearchInput, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockFileColFaissRepo.Setup(x => x.ByUserAndCollectionId((Guid)currentUser.Id!, simSearchInput.CollectionId))
            .ReturnsAsync(new DbGetOneResult<FileCollectionFaiss>(fileCollectionFaiss));
        
        //Act
        var act = () => _service.SimilaritySearch(simSearchInput, currentUser);
        
        //Assert
        await Assert.ThrowsAsync<ApiException>(act);
        
        _mockValidator.Verify(v => v.ValidateAsync(simSearchInput, It.IsAny<CancellationToken>()), Times.Once);
        _mockFileColFaissRepo.Verify(x => x.ByUserAndCollectionId((Guid)currentUser.Id!, simSearchInput.CollectionId),
            Times.Once);
        _mockSimSearchClient.Verify(x => x.TryInvokeAsync(It.IsAny<CoreSimilaritySearchInput>(), It.IsAny<CancellationToken>()), Times.Never);
        
    }
    [Fact]
    public async Task SimilaritySearch_With_No_Collection_Id_Should_Search_If_Authorized_User()
    {
        //Arrange
        var currentUser = _fixture.Build<Domain.Models.User>()
            .With(x => x.Id, Guid.NewGuid())
            .Create();
        var testFaissStuff = await TestFaissUtils.GetTestFaissStoreAsync();
        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, testFaissStuff.DocStore)
            .With(x => x.FaissIndex, testFaissStuff.FaissIndex)
            .With(x => x.UserId, currentUser.Id)
            .Create();

        var simSearchInput = _fixture
            .Build<SimilaritySearchInput>()
            .With(x => x.CollectionId, (Guid?)null)
            .Create();
        _mockValidator.Setup(v => v.ValidateAsync(simSearchInput, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockFileColFaissRepo.Setup(x => x.ByUserAndCollectionId((Guid)currentUser.Id!, simSearchInput.CollectionId))
            .ReturnsAsync(new DbGetOneResult<FileCollectionFaiss>(fileCollectionFaiss));
        _mockSimSearchClient
            .Setup(x => x
                .TryInvokeAsync(
                    It.Is<CoreSimilaritySearchInput>(y => 
                        y.FileInput == fileCollectionFaiss.FaissIndex &&
                        y.Question == simSearchInput.Question &&
                        y.DocumentsToReturn == simSearchInput.DocumentsToReturn &&
                        y.DocStore == fileCollectionFaiss.FaissJson
                    ),
                    It.IsAny<CancellationToken>()
                )
            ).ReturnsAsync(new CoreSimilaritySearchResponse
            {
                Items = [_fixture
                             .Build<SimilaritySearchResponseItem>()
                             .With(x => x.Metadata, new Dictionary<string, string>
                             {
                                 {"Id", testFaissStuff.SingleDocuments.FirstOrDefault()?.Id.ToString() ?? string.Empty}
                             })
                             .Create()
                ]
            });

        
        
        //Act
        await _service.SimilaritySearch(simSearchInput, currentUser);
        
        //Assert
        _mockValidator.Verify(v => v.ValidateAsync(simSearchInput, It.IsAny<CancellationToken>()), Times.Once);
        _mockFileColFaissRepo.Verify(x => x.ByUserAndCollectionId((Guid)currentUser.Id!, simSearchInput.CollectionId),
            Times.Once);
        _mockSimSearchClient.Verify(x => x
            .TryInvokeAsync(
                It.Is<CoreSimilaritySearchInput>(y => 
                    y.FileInput == fileCollectionFaiss.FaissIndex &&
                    y.Question == simSearchInput.Question &&
                    y.DocumentsToReturn == simSearchInput.DocumentsToReturn &&
                    y.DocStore == fileCollectionFaiss.FaissJson
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
    }
    [Fact]
    public async Task SimilaritySearch_With_Collection_Id_Should_Search_If_Authorized_User()
    {
        //Arrange
        var fileCollectionId = Guid.NewGuid();
        var currentUser = _fixture.Build<Domain.Models.User>()
            .With(x => x.Id, Guid.NewGuid())
            .Create();
        var testFaissStuff = await TestFaissUtils.GetTestFaissStoreAsync();
        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, testFaissStuff.DocStore)
            .With(x => x.FaissIndex, testFaissStuff.FaissIndex)
            .With(x => x.UserId, Guid.NewGuid())
            .With(x => x.CollectionId, fileCollectionId)
            .Create();
        var fileCollection = _fixture
            .Build<FileCollection>()
            .With(x => x.Id, fileCollectionId)
            .With(x => x.UserId, currentUser.Id)
            .With(x => x.FaissStore, fileCollectionFaiss)
            .Create();
        var simSearchInput = _fixture
            .Build<SimilaritySearchInput>()
            .With(x => x.CollectionId, fileCollection.Id)
            .Create();
        _mockValidator.Setup(v => v.ValidateAsync(simSearchInput, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockFileColRepo.Setup(x => x.GetOne((Guid)fileCollection.Id!, nameof(FileCollectionEntity.FaissStore), 
                nameof(FileCollectionEntity.SharedFileCollectionMembers)))
            .ReturnsAsync(new DbGetOneResult<FileCollection>(fileCollection));
        _mockSimSearchClient
            .Setup(x => x
                .TryInvokeAsync(
                    It.Is<CoreSimilaritySearchInput>(y => 
                        y.FileInput == fileCollectionFaiss.FaissIndex &&
                        y.Question == simSearchInput.Question &&
                        y.DocumentsToReturn == simSearchInput.DocumentsToReturn &&
                        y.DocStore == fileCollectionFaiss.FaissJson
                    ),
                    It.IsAny<CancellationToken>()
                )
            ).ReturnsAsync(new CoreSimilaritySearchResponse
            {
                Items = [_fixture
                             .Build<SimilaritySearchResponseItem>()
                             .With(x => x.Metadata, new Dictionary<string, string>
                             {
                                 {"Id", testFaissStuff.SingleDocuments.FirstOrDefault()?.Id.ToString() ?? string.Empty}
                             })
                             .Create()
                ]
            });

        
        
        //Act
        await _service.SimilaritySearch(simSearchInput, currentUser);
        
        //Assert
        _mockValidator.Verify(v => v.ValidateAsync(simSearchInput, It.IsAny<CancellationToken>()), Times.Once);
        _mockFileColRepo.Verify(x => x.GetOne((Guid)fileCollection.Id!, nameof(FileCollectionEntity.FaissStore),
            nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once);
        _mockSimSearchClient.Verify(x => x
            .TryInvokeAsync(
                It.Is<CoreSimilaritySearchInput>(y => 
                    y.FileInput == fileCollectionFaiss.FaissIndex &&
                    y.Question == simSearchInput.Question &&
                    y.DocumentsToReturn == simSearchInput.DocumentsToReturn &&
                    y.DocStore == fileCollectionFaiss.FaissJson
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
    }
    [Fact]
    public async Task SimilaritySearch_With_Collection_Id_Should_Search_If_Shared_Member()
    {
        //Arrange
        var fileCollectionId = Guid.NewGuid();
        var diffUserId = Guid.NewGuid();
        var currentUser = _fixture.Build<Domain.Models.User>()
            .With(x => x.Id, Guid.NewGuid())
            .Create();
        var testFaissStuff = await TestFaissUtils.GetTestFaissStoreAsync();
        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, testFaissStuff.DocStore)
            .With(x => x.FaissIndex, testFaissStuff.FaissIndex)
            .With(x => x.UserId, diffUserId)
            .With(x => x.CollectionId, fileCollectionId)
            .Create();
        var fileCollection = _fixture
            .Build<FileCollection>()
            .With(x => x.Id, fileCollectionId)
            .With(x => x.UserId, diffUserId)
            .With(x => x.FaissStore, fileCollectionFaiss)
            .With(x => x.SharedFileCollectionMembers, [
                _fixture
                    .Build<SharedFileCollectionMember>()
                    .With(x => x.CollectionId, fileCollectionId)
                    .With(x => x.UserId, currentUser.Id)
                    .With(x => x.CanSimilaritySearch, true)
                    .Create()
            ])
            .Create();
        var simSearchInput = _fixture
            .Build<SimilaritySearchInput>()
            .With(x => x.CollectionId, fileCollection.Id)
            .Create();
        _mockValidator.Setup(v => v.ValidateAsync(simSearchInput, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockFileColRepo.Setup(x => x.GetOne((Guid)fileCollection.Id!, nameof(FileCollectionEntity.FaissStore), 
                nameof(FileCollectionEntity.SharedFileCollectionMembers)))
            .ReturnsAsync(new DbGetOneResult<FileCollection>(fileCollection));
        _mockSimSearchClient
            .Setup(x => x
                .TryInvokeAsync(
                    It.Is<CoreSimilaritySearchInput>(y => 
                        y.FileInput == fileCollectionFaiss.FaissIndex &&
                        y.Question == simSearchInput.Question &&
                        y.DocumentsToReturn == simSearchInput.DocumentsToReturn &&
                        y.DocStore == fileCollectionFaiss.FaissJson
                    ),
                    It.IsAny<CancellationToken>()
                )
            ).ReturnsAsync(new CoreSimilaritySearchResponse
            {
                Items = [_fixture
                             .Build<SimilaritySearchResponseItem>()
                             .With(x => x.Metadata, new Dictionary<string, string>
                             {
                                 {"Id", testFaissStuff.SingleDocuments.FirstOrDefault()?.Id.ToString() ?? string.Empty}
                             })
                             .Create()
                ]
            });

        
        
        //Act
        await _service.SimilaritySearch(simSearchInput, currentUser);
        
        //Assert
        _mockValidator.Verify(v => v.ValidateAsync(simSearchInput, It.IsAny<CancellationToken>()), Times.Once);
        _mockFileColRepo.Verify(x => x.GetOne((Guid)fileCollection.Id!, nameof(FileCollectionEntity.FaissStore),
            nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once);
        _mockSimSearchClient.Verify(x => x
            .TryInvokeAsync(
                It.Is<CoreSimilaritySearchInput>(y => 
                    y.FileInput == fileCollectionFaiss.FaissIndex &&
                    y.Question == simSearchInput.Question &&
                    y.DocumentsToReturn == simSearchInput.DocumentsToReturn &&
                    y.DocStore == fileCollectionFaiss.FaissJson
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
    }
    
    [Fact]
    public async Task SimilaritySearch_With_Collection_Id_Should_Throw_If_Shared_Member_Not_Got_Perms()
    {
        //Arrange
        var fileCollectionId = Guid.NewGuid();
        var diffUserId = Guid.NewGuid();
        var currentUser = _fixture.Build<Domain.Models.User>()
            .With(x => x.Id, Guid.NewGuid())
            .Create();
        var testFaissStuff = await TestFaissUtils.GetTestFaissStoreAsync();
        var fileCollectionFaiss = _fixture.Build<FileCollectionFaiss>()
            .With(x => x.FaissJson, testFaissStuff.DocStore)
            .With(x => x.FaissIndex, testFaissStuff.FaissIndex)
            .With(x => x.UserId, diffUserId)
            .With(x => x.CollectionId, fileCollectionId)
            .Create();
        var fileCollection = _fixture
            .Build<FileCollection>()
            .With(x => x.Id, fileCollectionId)
            .With(x => x.UserId, diffUserId)
            .With(x => x.FaissStore, fileCollectionFaiss)
            .With(x => x.SharedFileCollectionMembers, [
                _fixture
                    .Build<SharedFileCollectionMember>()
                    .With(x => x.CollectionId, fileCollectionId)
                    .With(x => x.UserId, currentUser.Id)
                    .With(x => x.CanSimilaritySearch, false)
                    .Create()
            ])
            .Create();
        var simSearchInput = _fixture
            .Build<SimilaritySearchInput>()
            .With(x => x.CollectionId, fileCollection.Id)
            .Create();
        _mockValidator.Setup(v => v.ValidateAsync(simSearchInput, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockFileColRepo.Setup(x => x.GetOne((Guid)fileCollection.Id!, nameof(FileCollectionEntity.FaissStore), 
                nameof(FileCollectionEntity.SharedFileCollectionMembers)))
            .ReturnsAsync(new DbGetOneResult<FileCollection>(fileCollection));

        
        
        //Act
        var act = () => _service.SimilaritySearch(simSearchInput, currentUser);
        
        //Assert
        var ex = await Assert.ThrowsAsync<ApiException>(act);
        Assert.Equal(ExceptionConstants.Unauthorized, ex.Message);
        
        _mockValidator.Verify(v => v.ValidateAsync(simSearchInput, It.IsAny<CancellationToken>()), Times.Once);
        _mockFileColRepo.Verify(x => x.GetOne((Guid)fileCollection.Id!, nameof(FileCollectionEntity.FaissStore),
            nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once);
        _mockSimSearchClient.Verify(x => x
            .TryInvokeAsync(
                It.IsAny<CoreSimilaritySearchInput>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);
    }
}