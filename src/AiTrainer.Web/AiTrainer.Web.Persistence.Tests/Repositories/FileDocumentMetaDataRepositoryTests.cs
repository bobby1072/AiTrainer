using AutoFixture;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiTrainer.Web.Persistence.Tests.Repositories;

public sealed class FileDocumentMetaDataRepositoryTests : IDisposable
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ILogger<FileDocumentMetaDataRepository>> _mockLogger = new();
    private readonly DbContextOptions<AiTrainerContext> _dbContextOptions;
    private readonly Mock<IDbContextFactory<AiTrainerContext>> _mockDbContextFactory = new();

    public FileDocumentMetaDataRepositoryTests()
    {
        // Setup in-memory database
        _dbContextOptions = new DbContextOptionsBuilder<AiTrainerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(CoreEventId.InvalidIncludePathError)
                                    .Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        // Setup mock factory to return new context instances
        _mockDbContextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult(new AiTrainerContext(_dbContextOptions)));
    }

    private FileDocumentMetaDataRepository CreateSut() => new(_mockDbContextFactory.Object, _mockLogger.Object);

    private FileDocumentMetaData CreateValidFileDocumentMetaData(Guid? documentId = null)
    {
        return new FileDocumentMetaData
        {
            Id = _fixture.Create<long>(),
            DocumentId = documentId ?? Guid.NewGuid(),
            Title = _fixture.Create<string>(),
            Author = _fixture.Create<string>(),
            Subject = _fixture.Create<string>(),
            Keywords = _fixture.Create<string>(),
            Creator = _fixture.Create<string>(),
            Producer = _fixture.Create<string>(),
            CreationDate = _fixture.Create<string>(),
            ModifiedDate = _fixture.Create<string>(),
            NumberOfPages = _fixture.Create<int>(),
            IsEncrypted = _fixture.Create<bool>(),
            ExtraData = new Dictionary<string, string?> { { "TestKey", "TestValue" } }
        };
    }

    private FileDocumentMetaDataEntity CreateValidFileDocumentMetaDataEntity(Guid? documentId = null)
    {
        return new FileDocumentMetaDataEntity
        {
            Id = _fixture.Create<long>(),
            DocumentId = documentId ?? Guid.NewGuid(),
            Title = _fixture.Create<string>(),
            Author = _fixture.Create<string>(),
            Subject = _fixture.Create<string>(),
            Keywords = _fixture.Create<string>(),
            Creator = _fixture.Create<string>(),
            Producer = _fixture.Create<string>(),
            CreationDate = _fixture.Create<string>(),
            ModifiedDate = _fixture.Create<string>(),
            NumberOfPages = _fixture.Create<int>(),
            IsEncrypted = _fixture.Create<bool>(),
            ExtraData = new Dictionary<string, string?> { { "TestKey", "TestValue" } }
        };
    }

    [Fact]
    public async Task GetCount_ReturnsCorrectCount()
    {
        // Arrange
        var sut = CreateSut();
        var metaDataEntities = new[]
        {
            CreateValidFileDocumentMetaDataEntity(),
            CreateValidFileDocumentMetaDataEntity(),
            CreateValidFileDocumentMetaDataEntity()
        };
        
        await SeedFileDocumentMetaDataAsync(metaDataEntities);

        // Act
        var result = await sut.GetCount();

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(3, result.Data);
    }

    [Fact]
    public async Task GetCount_EmptyDatabase_ReturnsZero()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = await sut.GetCount();

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(0, result.Data);
    }

    [Fact]
    public async Task GetOne_ValidId_ReturnsFileDocumentMetaData()
    {
        // Arrange
        var sut = CreateSut();
        var metaDataEntity = CreateValidFileDocumentMetaDataEntity();
        
        await SeedFileDocumentMetaDataAsync(metaDataEntity);

        // Act
        var result = await sut.GetOne(metaDataEntity.Id);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Equal(metaDataEntity.Id, result.Data.Id);
        Assert.Equal(metaDataEntity.DocumentId, result.Data.DocumentId);
        Assert.Equal(metaDataEntity.Title, result.Data.Title);
        Assert.Equal(metaDataEntity.Author, result.Data.Author);
    }

    [Fact]
    public async Task GetOne_InvalidId_ReturnsNull()
    {
        // Arrange
        var sut = CreateSut();
        var nonExistentId = _fixture.Create<long>();

        // Act
        var result = await sut.GetOne(nonExistentId);

        // Assert
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetMany_ByIds_ReturnsMatchingMetaData()
    {
        // Arrange
        var sut = CreateSut();
        var metaDataEntities = new[]
        {
            CreateValidFileDocumentMetaDataEntity(),
            CreateValidFileDocumentMetaDataEntity(),
            CreateValidFileDocumentMetaDataEntity()
        };
        
        await SeedFileDocumentMetaDataAsync(metaDataEntities);

        var idsToFind = metaDataEntities.Take(2).Select(m => m.Id).ToArray();

        // Act
        var result = await sut.GetMany(idsToFind);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        Assert.All(result.Data, metaData => Assert.Contains(metaData.Id!.Value, idsToFind));
    }

    [Fact]
    public async Task GetMany_ByDocumentId_ReturnsMatchingMetaData()
    {
        // Arrange
        var sut = CreateSut();
        var documentId = Guid.NewGuid();
        var metaDataEntities = new[]
        {
            CreateValidFileDocumentMetaDataEntity(documentId),
            CreateValidFileDocumentMetaDataEntity(documentId),
            CreateValidFileDocumentMetaDataEntity() // Different document
        };
        
        await SeedFileDocumentMetaDataAsync(metaDataEntities);

        // Act
        var result = await sut.GetMany(documentId, nameof(FileDocumentMetaDataEntity.DocumentId));

        // Assert
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        Assert.All(result.Data, metaData => Assert.Equal(documentId, metaData.DocumentId));
    }

    [Fact]
    public async Task GetMany_ByTitle_ReturnsMatchingMetaData()
    {
        // Arrange
        var sut = CreateSut();
        var specificTitle = "TestTitle";
        var metaDataEntities = new[]
        {
            CreateValidFileDocumentMetaDataEntity(),
            CreateValidFileDocumentMetaDataEntity(),
            CreateValidFileDocumentMetaDataEntity()
        };
        
        // Set specific title for first two entities
        metaDataEntities[0] = metaDataEntities[0] with { Title = specificTitle };
        metaDataEntities[1] = metaDataEntities[1] with { Title = specificTitle };
        
        await SeedFileDocumentMetaDataAsync(metaDataEntities);

        // Act
        var result = await sut.GetMany(specificTitle, nameof(FileDocumentMetaDataEntity.Title));

        // Assert
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        Assert.All(result.Data, metaData => Assert.Equal(specificTitle, metaData.Title));
    }

    [Fact]
    public async Task GetMany_EmptyIds_ReturnsEmpty()
    {
        // Arrange
        var sut = CreateSut();
        var emptyIds = Array.Empty<long>();

        // Act
        var result = await sut.GetMany(emptyIds);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Create_ValidFileDocumentMetaData_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var metaData = CreateValidFileDocumentMetaData();

        // Act
        var result = await sut.Create([metaData]);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Single(result.Data);
        Assert.Equal(metaData.Title, result.Data.First().Title);
        Assert.Equal(metaData.Author, result.Data.First().Author);
        
        // Verify it was actually saved to database
        var savedEntity = await GetFileDocumentMetaDataFromDbAsync(metaData.Id!.Value);
        Assert.NotNull(savedEntity);
        Assert.Equal(metaData.Title, savedEntity.Title);
        Assert.Equal(metaData.Author, savedEntity.Author);
    }

    [Fact]
    public async Task Create_MultipleFileDocumentMetaData_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var metaDataList = new[]
        {
            CreateValidFileDocumentMetaData(),
            CreateValidFileDocumentMetaData(),
            CreateValidFileDocumentMetaData()
        };

        // Act
        var result = await sut.Create(metaDataList);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(3, result.Data.Count);
        
        // Verify all were saved to database
        var savedEntities = await GetAllFileDocumentMetaDataFromDbAsync();
        Assert.Equal(3, savedEntities.Count);
    }

    [Fact]
    public async Task Create_EmptyCollection_ReturnsEmptyResult()
    {
        // Arrange
        var sut = CreateSut();
        var emptyMetaData = Array.Empty<FileDocumentMetaData>();

        // Act
        var result = await sut.Create(emptyMetaData);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Update_ExistingFileDocumentMetaData_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var metaDataEntity = CreateValidFileDocumentMetaDataEntity();
        
        await SeedFileDocumentMetaDataAsync(metaDataEntity);

        var updatedMetaData = new FileDocumentMetaData
        {
            Id = metaDataEntity.Id,
            DocumentId = metaDataEntity.DocumentId,
            Title = "Updated Title",
            Author = "Updated Author",
            Subject = metaDataEntity.Subject,
            Keywords = metaDataEntity.Keywords,
            Creator = metaDataEntity.Creator,
            Producer = metaDataEntity.Producer,
            CreationDate = metaDataEntity.CreationDate,
            ModifiedDate = metaDataEntity.ModifiedDate,
            NumberOfPages = metaDataEntity.NumberOfPages,
            IsEncrypted = metaDataEntity.IsEncrypted,
            ExtraData = metaDataEntity.ExtraData ?? []
        };

        // Act
        var result = await sut.Update([updatedMetaData]);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Single(result.Data);
        Assert.Equal("Updated Title", result.Data.First().Title);
        Assert.Equal("Updated Author", result.Data.First().Author);
        
        // Verify the update in database
        var updatedEntity = await GetFileDocumentMetaDataFromDbAsync(metaDataEntity.Id);
        Assert.NotNull(updatedEntity);
        Assert.Equal("Updated Title", updatedEntity.Title);
        Assert.Equal("Updated Author", updatedEntity.Author);
    }

    [Fact]
    public async Task Update_NonExistentFileDocumentMetaData_ThrowsException()
    {
        // Arrange
        var sut = CreateSut();
        var nonExistentMetaData = CreateValidFileDocumentMetaData();

        // Act & Assert - Updating non-existent entity throws exception
        await Assert.ThrowsAnyAsync<Exception>(() => sut.Update([nonExistentMetaData]));
    }

    [Fact]
    public async Task Delete_ExistingFileDocumentMetaData_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var metaDataEntities = new[]
        {
            CreateValidFileDocumentMetaDataEntity(),
            CreateValidFileDocumentMetaDataEntity()
        };
        
        await SeedFileDocumentMetaDataAsync(metaDataEntities);

        var metaDataToDelete = metaDataEntities.Select(e => e.ToModel()).ToArray();

        // Act
        var result = await sut.Delete(metaDataToDelete);

        // Assert
        Assert.Equal(metaDataToDelete, result.Data);
        
        // Verify deletion in database
        var remainingEntities = await GetAllFileDocumentMetaDataFromDbAsync();
        Assert.Empty(remainingEntities);
    }

    [Fact]
    public async Task Delete_ByIds_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var metaDataEntities = new[]
        {
            CreateValidFileDocumentMetaDataEntity(),
            CreateValidFileDocumentMetaDataEntity()
        };
        
        await SeedFileDocumentMetaDataAsync(metaDataEntities);

        var idsToDelete = metaDataEntities.Select(e => e.Id).ToArray();

        // Act
        var result = await sut.Delete(idsToDelete);

        // Assert
        Assert.Equal(idsToDelete, result.Data);
        
        // Verify deletion in database
        var remainingEntities = await GetAllFileDocumentMetaDataFromDbAsync();
        Assert.Empty(remainingEntities);
    }

    [Fact]
    public async Task Exists_ExistingFileDocumentMetaData_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSut();
        var metaDataEntity = CreateValidFileDocumentMetaDataEntity();
        
        await SeedFileDocumentMetaDataAsync(metaDataEntity);

        // Act
        var result = await sut.Exists(metaDataEntity.Id);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task Exists_NonExistentFileDocumentMetaData_ReturnsFalse()
    {
        // Arrange
        var sut = CreateSut();
        var nonExistentId = _fixture.Create<long>();

        // Act
        var result = await sut.Exists(nonExistentId);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.False(result.Data);
    }

    [Fact]
    public async Task Exists_ByTitle_ExistingMetaData_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSut();
        var metaDataEntity = CreateValidFileDocumentMetaDataEntity();
        
        await SeedFileDocumentMetaDataAsync(metaDataEntity);

        // Act
        var result = await sut.Exists(metaDataEntity.Title, nameof(FileDocumentMetaDataEntity.Title));

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task Exists_ByTitle_NonExistentMetaData_ReturnsFalse()
    {
        // Arrange
        var sut = CreateSut();
        var nonExistentTitle = "NonExistentTitle";

        // Act
        var result = await sut.Exists(nonExistentTitle, nameof(FileDocumentMetaDataEntity.Title));

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.False(result.Data);
    }

    [Fact]
    public async Task Exists_ByDocumentId_ExistingMetaData_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSut();
        var documentId = Guid.NewGuid();
        var metaDataEntity = CreateValidFileDocumentMetaDataEntity(documentId);
        
        await SeedFileDocumentMetaDataAsync(metaDataEntity);

        // Act
        var result = await sut.Exists(documentId, nameof(FileDocumentMetaDataEntity.DocumentId));

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task Exists_ByDocumentId_NonExistentMetaData_ReturnsFalse()
    {
        // Arrange
        var sut = CreateSut();
        var nonExistentDocumentId = Guid.NewGuid();

        // Act
        var result = await sut.Exists(nonExistentDocumentId, nameof(FileDocumentMetaDataEntity.DocumentId));

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.False(result.Data);
    }

    private async Task SeedFileDocumentMetaDataAsync(params FileDocumentMetaDataEntity[] metaDataEntities)
    {
        await using var context = new AiTrainerContext(_dbContextOptions);
        await context.FileDocumentMetaData.AddRangeAsync(metaDataEntities);
        await context.SaveChangesAsync();
    }

    private async Task<FileDocumentMetaDataEntity?> GetFileDocumentMetaDataFromDbAsync(long id)
    {
        await using var context = new AiTrainerContext(_dbContextOptions);
        return await context.FileDocumentMetaData.FirstOrDefaultAsync(m => m.Id == id);
    }

    private async Task<List<FileDocumentMetaDataEntity>> GetAllFileDocumentMetaDataFromDbAsync()
    {
        await using var context = new AiTrainerContext(_dbContextOptions);
        return await context.FileDocumentMetaData.ToListAsync();
    }

    public void Dispose()
    {
        // Nothing to dispose since we create new contexts for each operation
    }
}
