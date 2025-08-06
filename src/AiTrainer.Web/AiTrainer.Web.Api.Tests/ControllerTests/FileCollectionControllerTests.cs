using System.Security.Claims;
using System.Text.Json;
using AiTrainer.Web.Api.Controllers;
using AiTrainer.Web.Api.Models;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Partials;
using AiTrainer.Web.Domain.Models.Views;
using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AiTrainer.Web.Api.Tests.ControllerTests;

public sealed class FileCollectionControllerTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IHttpDomainServiceActionExecutor> _mockActionExecutor = new();

    public FileCollectionControllerTests()
    {
        // Configure AutoFixture to handle problematic types
        _fixture.Register(() => JsonDocument.Parse("{}"));
    }

    private FileCollectionController CreateSut()
    {
        var controller = new FileCollectionController(_mockActionExecutor.Object, new NullLogger<FileCollectionController>());
        
        // Setup HttpContext with user claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new("access_token", "test-token")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
        
        return controller;
    }

    [Fact]
    public async Task Share_UnshareWithMember_ReturnsSuccessfulOutcome()
    {
        // Arrange
        var sut = CreateSut();
        var input = _fixture.Create<RequiredGuidIdInput>();
        var currentUser = _fixture.Create<User>();
        var expectedResult = Guid.NewGuid();
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IUserProcessingManager, User?>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IUserProcessingManager, Task<User?>>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(currentUser);
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IFileCollectionProcessingManager, Guid>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IFileCollectionProcessingManager, Task<Guid>>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await sut.Share(input);

        // Assert
        Assert.IsType<Outcome<Guid>>(result.Value);
        var outcome = result.Value;
        Assert.True(outcome.IsSuccess);
        Assert.Equal(expectedResult, outcome.Data);
        
        _mockActionExecutor.Verify(x => x.ExecuteAsync<IFileCollectionProcessingManager, Guid>(
            It.IsAny<System.Linq.Expressions.Expression<Func<IFileCollectionProcessingManager, Task<Guid>>>>(),
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Share_ShareWithMembers_ReturnsSuccessfulOutcome()
    {
        // Arrange
        var sut = CreateSut();
        var collectionId = Guid.NewGuid();
        var currentUser = _fixture.Create<User>();
        var expectedMembers = _fixture.CreateMany<SharedFileCollectionMember>(2).ToArray();
        
        // Create proper SharedFileCollectionMemberRawSaveInput
        var jsonDocuments = new[]
        {
            JsonDocument.Parse($$"""
            {
                "email": "test@example.com",
                "userId": "",
                "canViewDocuments": true,
                "canDownloadDocuments": false,
                "canCreateDocuments": false,
                "canRemoveDocuments": false,
                "canSimilaritySearch": true
            }
            """),
            JsonDocument.Parse($$"""
            {
                "email": "",
                "userId": "{{Guid.NewGuid()}}",
                "canViewDocuments": false,
                "canDownloadDocuments": true,
                "canCreateDocuments": true,
                "canRemoveDocuments": false,
                "canSimilaritySearch": false
            }
            """)
        };
        
        var input = new SharedFileCollectionMemberRawSaveInput
        {
            CollectionId = collectionId,
            MembersToShareTo = jsonDocuments
        };
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IUserProcessingManager, User?>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IUserProcessingManager, Task<User?>>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(currentUser);
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IFileCollectionProcessingManager, IReadOnlyCollection<SharedFileCollectionMember>>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IFileCollectionProcessingManager, Task<IReadOnlyCollection<SharedFileCollectionMember>>>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(expectedMembers);

        // Act
        var result = await sut.Share(input);

        // Assert
        Assert.IsType<Outcome<IReadOnlyCollection<SharedFileCollectionMember>>>(result.Value);
        var outcome = result.Value;
        Assert.True(outcome.IsSuccess);
        Assert.Equal(expectedMembers, outcome.Data);
        
        _mockActionExecutor.Verify(x => x.ExecuteAsync<IFileCollectionProcessingManager, IReadOnlyCollection<SharedFileCollectionMember>>(
            It.IsAny<System.Linq.Expressions.Expression<Func<IFileCollectionProcessingManager, Task<IReadOnlyCollection<SharedFileCollectionMember>>>>>(),
            nameof(IFileCollectionProcessingManager.ShareFileCollectionAsync)), Times.Once);
    }

    [Fact]
    public async Task Share_ShareWithMembers_InvalidJson_ThrowsApiException()
    {
        // Arrange
        var sut = CreateSut();
        var currentUser = _fixture.Create<User>();
        
        // Create invalid JSON input - missing required properties
        var invalidJsonDocument = JsonDocument.Parse("""
        {
            "invalid": "structure"
        }
        """);
        
        var input = new SharedFileCollectionMemberRawSaveInput
        {
            CollectionId = Guid.NewGuid(),
            MembersToShareTo = [invalidJsonDocument]
        };
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IUserProcessingManager, User?>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IUserProcessingManager, Task<User?>>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(currentUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => sut.Share(input));
        Assert.Contains("Failed to deserialize json input", exception.Message);
    }

    [Fact]
    public async Task Download_ValidInput_ReturnsZipFile()
    {
        // Arrange
        var sut = CreateSut();
        var input = _fixture.Create<RequiredGuidIdInput>();
        var currentUser = _fixture.Create<User>();
        
        // Create FileCollection manually to avoid AutoFixture JsonDocument issues
        var fileCollection = new FileCollection
        {
            Id = Guid.NewGuid(),
            CollectionName = "TestCollection",
            DateCreated = DateTime.UtcNow,
            DateModified = DateTime.UtcNow,
            Documents = new[]
            {
                new FileDocument
                {
                    Id = Guid.NewGuid(),
                    FileName = "test1.txt",
                    FileData = "Hello World"u8.ToArray(),
                    FileType = FileTypeEnum.Text,
                    DateCreated = DateTime.UtcNow,
                    UserId = currentUser.Id ?? Guid.NewGuid(),
                    CollectionId = Guid.NewGuid()
                },
                new FileDocument
                {
                    Id = Guid.NewGuid(),
                    FileName = "test2.txt",
                    FileData = "Another file"u8.ToArray(),
                    FileType = FileTypeEnum.Text,
                    DateCreated = DateTime.UtcNow,
                    UserId = currentUser.Id ?? Guid.NewGuid(),
                    CollectionId = Guid.NewGuid()
                }
            },
            UserId = currentUser.Id ?? Guid.NewGuid()
        };
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IUserProcessingManager, User?>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IUserProcessingManager, Task<User?>>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(currentUser);
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IFileCollectionProcessingManager, FileCollection>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IFileCollectionProcessingManager, Task<FileCollection>>>>(),
                nameof(IFileCollectionProcessingManager.GetFileCollectionWithContentsAsync)))
            .ReturnsAsync(fileCollection);

        // Act
        var result = await sut.Download(input);

        // Assert
        Assert.IsAssignableFrom<FileResult>(result);
        var fileResult = (FileResult)result;
        Assert.Equal("application/zip", fileResult.ContentType);
        Assert.Equal("TestCollection.zip", fileResult.FileDownloadName);
        
        _mockActionExecutor.Verify(x => x.ExecuteAsync<IFileCollectionProcessingManager, FileCollection>(
            It.IsAny<System.Linq.Expressions.Expression<Func<IFileCollectionProcessingManager, Task<FileCollection>>>>(),
            nameof(IFileCollectionProcessingManager.GetFileCollectionWithContentsAsync)), Times.Once);
    }

    [Fact]
    public async Task Download_EmptyDocuments_ReturnsEmptyZipFile()
    {
        // Arrange
        var sut = CreateSut();
        var input = _fixture.Create<RequiredGuidIdInput>();
        var currentUser = _fixture.Create<User>();
        
        var fileCollection = new FileCollection
        {
            Id = Guid.NewGuid(),
            CollectionName = "EmptyCollection",
            DateCreated = DateTime.UtcNow,
            DateModified = DateTime.UtcNow,
            Documents = null,
            UserId = currentUser.Id ?? Guid.NewGuid()
        };
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IUserProcessingManager, User?>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IUserProcessingManager, Task<User?>>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(currentUser);
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IFileCollectionProcessingManager, FileCollection>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IFileCollectionProcessingManager, Task<FileCollection>>>>(),
                nameof(IFileCollectionProcessingManager.GetFileCollectionWithContentsAsync)))
            .ReturnsAsync(fileCollection);

        // Act
        var result = await sut.Download(input);

        // Assert
        Assert.IsAssignableFrom<FileResult>(result);
        var fileResult = (FileResult)result;
        Assert.Equal("application/zip", fileResult.ContentType);
        Assert.Equal("EmptyCollection.zip", fileResult.FileDownloadName);
    }

    [Fact]
    public async Task SaveCollection_ValidInput_ReturnsSuccessfulOutcome()
    {
        // Arrange
        var sut = CreateSut();
        var input = _fixture.Create<FileCollectionSaveInput>();
        var currentUser = _fixture.Create<User>();
        var expectedCollection = new FileCollection
        {
            Id = Guid.NewGuid(),
            CollectionName = input.CollectionName,
            UserId = currentUser.Id ?? Guid.NewGuid(),
            DateCreated = DateTime.UtcNow,
            DateModified = DateTime.UtcNow
        };
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IUserProcessingManager, User?>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IUserProcessingManager, Task<User?>>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(currentUser);
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IFileCollectionProcessingManager, FileCollection>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IFileCollectionProcessingManager, Task<FileCollection>>>>(),
                nameof(IFileCollectionProcessingManager.SaveFileCollectionAsync)))
            .ReturnsAsync(expectedCollection);

        // Act
        var result = await sut.SaveCollection(input);

        // Assert
        Assert.IsType<Outcome<FileCollection>>(result.Value);
        var outcome = result.Value;
        Assert.True(outcome.IsSuccess);
        Assert.Equal(expectedCollection, outcome.Data);
        
        _mockActionExecutor.Verify(x => x.ExecuteAsync<IFileCollectionProcessingManager, FileCollection>(
            It.IsAny<System.Linq.Expressions.Expression<Func<IFileCollectionProcessingManager, Task<FileCollection>>>>(),
            nameof(IFileCollectionProcessingManager.SaveFileCollectionAsync)), Times.Once);
    }

    [Fact]
    public async Task GetOneLayer_WithCollectionId_ReturnsSuccessfulOutcome()
    {
        // Arrange
        var sut = CreateSut();
        var input = _fixture.Build<OptionalIdInput>()
            .With(x => x.Id, Guid.NewGuid())
            .Create();
        var currentUser = _fixture.Create<User>();
        var expectedView = new FlatFileDocumentPartialCollectionView
        {
            FileCollections = new List<FileCollection>(),
            FileDocuments = new List<FileDocumentPartial>()
        };
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IUserProcessingManager, User?>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IUserProcessingManager, Task<User?>>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(currentUser);
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IFileCollectionProcessingManager, FlatFileDocumentPartialCollectionView>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IFileCollectionProcessingManager, Task<FlatFileDocumentPartialCollectionView>>>>(),
                nameof(IFileCollectionProcessingManager.GetOneLayerFileDocPartialsAndCollectionsAsync)))
            .ReturnsAsync(expectedView);

        // Act
        var result = await sut.GetOneLayer(input);

        // Assert
        Assert.IsType<Outcome<FlatFileDocumentPartialCollectionView>>(result.Value);
        var outcome = result.Value;
        Assert.True(outcome.IsSuccess);
        Assert.Equal(expectedView, outcome.Data);
        
        _mockActionExecutor.Verify(x => x.ExecuteAsync<IFileCollectionProcessingManager, FlatFileDocumentPartialCollectionView>(
            It.IsAny<System.Linq.Expressions.Expression<Func<IFileCollectionProcessingManager, Task<FlatFileDocumentPartialCollectionView>>>>(),
            nameof(IFileCollectionProcessingManager.GetOneLayerFileDocPartialsAndCollectionsAsync)), Times.Once);
    }

    [Fact]
    public async Task GetOneLayer_WithoutCollectionId_ReturnsSuccessfulOutcome()
    {
        // Arrange
        var sut = CreateSut();
        var input = _fixture.Build<OptionalIdInput>()
            .With(x => x.Id, (Guid?)null)
            .Create();
        var currentUser = _fixture.Create<User>();
        var expectedView = new FlatFileDocumentPartialCollectionView
        {
            FileCollections = new List<FileCollection>(),
            FileDocuments = new List<FileDocumentPartial>()
        };
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IUserProcessingManager, User?>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IUserProcessingManager, Task<User?>>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(currentUser);
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IFileCollectionProcessingManager, FlatFileDocumentPartialCollectionView>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IFileCollectionProcessingManager, Task<FlatFileDocumentPartialCollectionView>>>>(),
                nameof(IFileCollectionProcessingManager.GetOneLayerFileDocPartialsAndCollectionsAsync)))
            .ReturnsAsync(expectedView);

        // Act
        var result = await sut.GetOneLayer(input);

        // Assert
        Assert.IsType<Outcome<FlatFileDocumentPartialCollectionView>>(result.Value);
        var outcome = result.Value;
        Assert.True(outcome.IsSuccess);
        Assert.Equal(expectedView, outcome.Data);
    }

    [Fact]
    public async Task Delete_ValidInput_ReturnsSuccessfulOutcome()
    {
        // Arrange
        var sut = CreateSut();
        var input = _fixture.Create<RequiredGuidIdInput>();
        var currentUser = _fixture.Create<User>();
        var expectedId = input.Id;
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IUserProcessingManager, User?>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IUserProcessingManager, Task<User?>>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(currentUser);
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IFileCollectionProcessingManager, Guid>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IFileCollectionProcessingManager, Task<Guid>>>>(),
                nameof(IFileCollectionProcessingManager.DeleteFileCollectionAsync)))
            .ReturnsAsync(expectedId);

        // Act
        var result = await sut.Delete(input);

        // Assert
        Assert.IsType<Outcome<Guid>>(result.Value);
        var outcome = result.Value;
        Assert.True(outcome.IsSuccess);
        Assert.Equal(expectedId, outcome.Data);
        
        _mockActionExecutor.Verify(x => x.ExecuteAsync<IFileCollectionProcessingManager, Guid>(
            It.IsAny<System.Linq.Expressions.Expression<Func<IFileCollectionProcessingManager, Task<Guid>>>>(),
            nameof(IFileCollectionProcessingManager.DeleteFileCollectionAsync)), Times.Once);
    }

    [Fact]
    public async Task GetCurrentUser_WhenUserNotFound_ThrowsApiException()
    {
        // Arrange
        var sut = CreateSut();
        var input = _fixture.Create<RequiredGuidIdInput>();
        
        _mockActionExecutor.Setup(x => x.ExecuteAsync<IUserProcessingManager, User?>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IUserProcessingManager, Task<User?>>>>(),
                It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => sut.Share(input));
        Assert.Equal("Not authorized to access this resource", exception.Message);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, exception.StatusCode);
    }
}
