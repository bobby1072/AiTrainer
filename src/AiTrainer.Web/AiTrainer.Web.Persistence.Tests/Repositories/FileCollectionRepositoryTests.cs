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

public sealed class FileCollectionRepositoryTests : IDisposable
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ILogger<FileCollectionRepository>> _mockLogger = new();
    private readonly DbContextOptions<AiTrainerContext> _dbContextOptions;
    private readonly Mock<IDbContextFactory<AiTrainerContext>> _mockDbContextFactory = new();

    public FileCollectionRepositoryTests()
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

    private FileCollectionRepository CreateSut() => new(_mockDbContextFactory.Object, _mockLogger.Object);

    private FileCollection CreateValidFileCollection(Guid? userId = null)
    {
        return new FileCollection
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            CollectionName = _fixture.Create<string>(),
            CollectionDescription = _fixture.Create<string>(),
            DateCreated = DateTime.UtcNow,
            DateModified = DateTime.UtcNow
        };
    }

    private FileCollectionEntity CreateValidFileCollectionEntity(Guid? userId = null)
    {
        return new FileCollectionEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            CollectionName = _fixture.Create<string>(),
            CollectionDescription = _fixture.Create<string>(),
            DateCreated = DateTime.UtcNow,
            DateModified = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task GetCount_ReturnsCorrectCount()
    {
        // Arrange
        var sut = CreateSut();
        var collections = new[]
        {
            CreateValidFileCollectionEntity(),
            CreateValidFileCollectionEntity(),
            CreateValidFileCollectionEntity()
        };
        
        await SeedFileCollectionsAsync(collections);

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
    public async Task GetOne_ValidId_ReturnsFileCollection()
    {
        // Arrange
        var sut = CreateSut();
        var collectionEntity = CreateValidFileCollectionEntity();
        
        await SeedFileCollectionsAsync(collectionEntity);

        // Act
        var result = await sut.GetOne(collectionEntity.Id);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Equal(collectionEntity.Id, result.Data.Id);
        Assert.Equal(collectionEntity.CollectionName, result.Data.CollectionName);
        Assert.Equal(collectionEntity.UserId, result.Data.UserId);
    }

    [Fact]
    public async Task GetOne_InvalidId_ReturnsNull()
    {
        // Arrange
        var sut = CreateSut();
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await sut.GetOne(nonExistentId);

        // Assert
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetOneByCollectionIdAndUserIdAsync_ValidIds_ReturnsFileCollection()
    {
        // Arrange
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var collectionEntity = CreateValidFileCollectionEntity(userId);
        
        await SeedFileCollectionsAsync(collectionEntity);

        // Act
        var result = await sut.GetOneByCollectionIdAndUserIdAsync(userId, collectionEntity.Id);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Equal(collectionEntity.Id, result.Data.Id);
        Assert.Equal(userId, result.Data.UserId);
    }

    [Fact]
    public async Task GetOneByCollectionIdAndUserIdAsync_WrongUserId_ReturnsNull()
    {
        // Arrange
        var sut = CreateSut();
        var userId = Guid.NewGuid();
        var wrongUserId = Guid.NewGuid();
        var collectionEntity = CreateValidFileCollectionEntity(userId);
        
        await SeedFileCollectionsAsync(collectionEntity);

        // Act
        var result = await sut.GetOneByCollectionIdAndUserIdAsync(wrongUserId, collectionEntity.Id);

        // Assert
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetMany_ByIds_ReturnsMatchingCollections()
    {
        // Arrange
        var sut = CreateSut();
        var collectionEntities = new[]
        {
            CreateValidFileCollectionEntity(),
            CreateValidFileCollectionEntity(),
            CreateValidFileCollectionEntity()
        };
        
        await SeedFileCollectionsAsync(collectionEntities);

        var idsToFind = collectionEntities.Take(2).Select(c => c.Id).ToArray();

        // Act
        var result = await sut.GetMany(idsToFind);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        Assert.All(result.Data, collection => Assert.Contains(collection.Id!.Value, idsToFind));
    }

    [Fact]
    public async Task GetMany_EmptyIds_ReturnsEmpty()
    {
        // Arrange
        var sut = CreateSut();
        var emptyIds = Array.Empty<Guid>();

        // Act
        var result = await sut.GetMany(emptyIds);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Create_ValidFileCollection_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var collection = CreateValidFileCollection();

        // Act
        var result = await sut.Create([collection]);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Single(result.Data);
        Assert.Equal(collection.CollectionName, result.Data.First().CollectionName);
        
        // Verify it was actually saved to database
        var savedEntity = await GetFileCollectionFromDbAsync(collection.Id!.Value);
        Assert.NotNull(savedEntity);
        Assert.Equal(collection.CollectionName, savedEntity.CollectionName);
    }

    [Fact]
    public async Task Create_MultipleFileCollections_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var collections = new[]
        {
            CreateValidFileCollection(),
            CreateValidFileCollection(),
            CreateValidFileCollection()
        };

        // Act
        var result = await sut.Create(collections);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(3, result.Data.Count);
        
        // Verify all were saved to database
        var savedEntities = await GetAllFileCollectionsFromDbAsync();
        Assert.Equal(3, savedEntities.Count);
    }

    [Fact]
    public async Task Create_EmptyCollection_ReturnsEmpty()
    {
        // Arrange
        var sut = CreateSut();
        var emptyCollections = Array.Empty<FileCollection>();

        // Act
        var result = await sut.Create(emptyCollections);

        // Assert - Empty collections return empty results but might still be successful
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Update_ExistingFileCollection_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var collectionEntity = CreateValidFileCollectionEntity();
        
        await SeedFileCollectionsAsync(collectionEntity);

        var updatedCollection = new FileCollection
        {
            Id = collectionEntity.Id,
            UserId = collectionEntity.UserId,
            CollectionName = "Updated Name",
            CollectionDescription = "Updated Description",
            DateCreated = collectionEntity.DateCreated,
            DateModified = DateTime.UtcNow
        };

        // Act
        var result = await sut.Update([updatedCollection]);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Single(result.Data);
        Assert.Equal("Updated Name", result.Data.First().CollectionName);
        
        // Verify the update in database
        var updatedEntity = await GetFileCollectionFromDbAsync(collectionEntity.Id);
        Assert.NotNull(updatedEntity);
        Assert.Equal("Updated Name", updatedEntity.CollectionName);
        Assert.Equal("Updated Description", updatedEntity.CollectionDescription);
    }

    [Fact]
    public async Task Update_NonExistentFileCollection_ThrowsException()
    {
        // Arrange
        var sut = CreateSut();
        var nonExistentCollection = CreateValidFileCollection();

        // Act & Assert - Updating non-existent entity throws exception
        await Assert.ThrowsAnyAsync<Exception>(() => sut.Update([nonExistentCollection]));
    }

    [Fact]
    public async Task Delete_ExistingFileCollections_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var collectionEntities = new[]
        {
            CreateValidFileCollectionEntity(),
            CreateValidFileCollectionEntity()
        };
        
        await SeedFileCollectionsAsync(collectionEntities);

        var collectionsToDelete = collectionEntities.Select(e => e.ToModel()).ToArray();

        // Act
        var result = await sut.Delete(collectionsToDelete);

        // Assert
        Assert.Equal(collectionsToDelete, result.Data);
        
        // Verify deletion in database
        var remainingEntities = await GetAllFileCollectionsFromDbAsync();
        Assert.Empty(remainingEntities);
    }

    [Fact]
    public async Task Delete_ByIds_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var collectionEntities = new[]
        {
            CreateValidFileCollectionEntity(),
            CreateValidFileCollectionEntity()
        };
        
        await SeedFileCollectionsAsync(collectionEntities);

        var idsToDelete = collectionEntities.Select(e => e.Id).ToArray();

        // Act
        var result = await sut.Delete(idsToDelete);

        // Assert
        Assert.Equal(idsToDelete, result.Data);
        
        // Verify deletion in database
        var remainingEntities = await GetAllFileCollectionsFromDbAsync();
        Assert.Empty(remainingEntities);
    }

    [Fact]
    public async Task Exists_ExistingFileCollection_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSut();
        var collectionEntity = CreateValidFileCollectionEntity();
        
        await SeedFileCollectionsAsync(collectionEntity);

        // Act
        var result = await sut.Exists(collectionEntity.Id);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task Exists_NonExistentFileCollection_ReturnsFalse()
    {
        // Arrange
        var sut = CreateSut();
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await sut.Exists(nonExistentId);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.False(result.Data);
    }

    [Fact]
    public async Task Exists_ByName_ExistingFileCollection_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSut();
        var collectionEntity = CreateValidFileCollectionEntity();
        
        await SeedFileCollectionsAsync(collectionEntity);

        // Act
        var result = await sut.Exists(collectionEntity.CollectionName, nameof(FileCollectionEntity.CollectionName));

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task Exists_ByName_NonExistentFileCollection_ReturnsFalse()
    {
        // Arrange
        var sut = CreateSut();
        var nonExistentName = "NonExistentCollection";

        // Act
        var result = await sut.Exists(nonExistentName, nameof(FileCollectionEntity.CollectionName));

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.False(result.Data);
    }

    private async Task SeedFileCollectionsAsync(params FileCollectionEntity[] collections)
    {
        await using var context = new AiTrainerContext(_dbContextOptions);
        await context.FileCollections.AddRangeAsync(collections);
        await context.SaveChangesAsync();
    }

    private async Task<FileCollectionEntity?> GetFileCollectionFromDbAsync(Guid id)
    {
        await using var context = new AiTrainerContext(_dbContextOptions);
        return await context.FileCollections.FirstOrDefaultAsync(c => c.Id == id);
    }

    private async Task<List<FileCollectionEntity>> GetAllFileCollectionsFromDbAsync()
    {
        await using var context = new AiTrainerContext(_dbContextOptions);
        return await context.FileCollections.ToListAsync();
    }

    public void Dispose()
    {
        // Nothing to dispose since we create new contexts for each operation
    }
}
