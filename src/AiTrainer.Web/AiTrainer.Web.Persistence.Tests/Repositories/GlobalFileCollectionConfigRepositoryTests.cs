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

public sealed class GlobalFileCollectionConfigRepositoryTests : IDisposable
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ILogger<GlobalFileCollectionConfigRepository>> _mockLogger = new();
    private readonly DbContextOptions<AiTrainerContext> _dbContextOptions;
    private readonly Mock<IDbContextFactory<AiTrainerContext>> _mockDbContextFactory = new();

    public GlobalFileCollectionConfigRepositoryTests()
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

    private GlobalFileCollectionConfigRepository CreateSut() => new(_mockLogger.Object, _mockDbContextFactory.Object);

    private GlobalFileCollectionConfig CreateValidGlobalFileCollectionConfig()
    {
        return new GlobalFileCollectionConfig
        {
            Id = _fixture.Create<long>(),
            AutoFaissSync = _fixture.Create<bool>(),
            UserId = Guid.NewGuid()
        };
    }

    private GlobalFileCollectionConfigEntity CreateValidGlobalFileCollectionConfigEntity()
    {
        return new GlobalFileCollectionConfigEntity
        {
            Id = _fixture.Create<long>(),
            AutoFaissSync = _fixture.Create<bool>(),
            UserId = Guid.NewGuid()
        };
    }

    [Fact]
    public async Task GetCount_ReturnsCorrectCount()
    {
        // Arrange
        var sut = CreateSut();
        var configs = new[]
        {
            CreateValidGlobalFileCollectionConfigEntity(),
            CreateValidGlobalFileCollectionConfigEntity(),
            CreateValidGlobalFileCollectionConfigEntity()
        };
        
        await SeedGlobalFileCollectionConfigsAsync(configs);

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
    public async Task GetOne_ValidId_ReturnsGlobalFileCollectionConfig()
    {
        // Arrange
        var sut = CreateSut();
        var configEntity = CreateValidGlobalFileCollectionConfigEntity();
        
        await SeedGlobalFileCollectionConfigsAsync(configEntity);

        // Act
        var result = await sut.GetOne(configEntity.Id);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Equal(configEntity.Id, result.Data.Id);
        Assert.Equal(configEntity.AutoFaissSync, result.Data.AutoFaissSync);
        Assert.Equal(configEntity.UserId, result.Data.UserId);
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
    public async Task GetMany_ByIds_ReturnsMatchingConfigs()
    {
        // Arrange
        var sut = CreateSut();
        var configEntities = new[]
        {
            CreateValidGlobalFileCollectionConfigEntity(),
            CreateValidGlobalFileCollectionConfigEntity(),
            CreateValidGlobalFileCollectionConfigEntity()
        };
        
        await SeedGlobalFileCollectionConfigsAsync(configEntities);

        var idsToFind = configEntities.Take(2).Select(c => c.Id).ToArray();

        // Act
        var result = await sut.GetMany(idsToFind);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        Assert.All(result.Data, config => Assert.Contains(config.Id!.Value, idsToFind));
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
    public async Task Create_ValidGlobalFileCollectionConfig_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var config = CreateValidGlobalFileCollectionConfig();

        // Act
        var result = await sut.Create([config]);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Single(result.Data);
        Assert.Equal(config.AutoFaissSync, result.Data.First().AutoFaissSync);
        Assert.Equal(config.UserId, result.Data.First().UserId);
        
        // Verify it was actually saved to database
        var savedEntity = await GetGlobalFileCollectionConfigFromDbAsync(config.Id!.Value);
        Assert.NotNull(savedEntity);
        Assert.Equal(config.AutoFaissSync, savedEntity.AutoFaissSync);
    }

    [Fact]
    public async Task Create_MultipleGlobalFileCollectionConfigs_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var configs = new[]
        {
            CreateValidGlobalFileCollectionConfig(),
            CreateValidGlobalFileCollectionConfig(),
            CreateValidGlobalFileCollectionConfig()
        };

        // Act
        var result = await sut.Create(configs);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(3, result.Data.Count);
        
        // Verify all were saved to database
        var savedEntities = await GetAllGlobalFileCollectionConfigsFromDbAsync();
        Assert.Equal(3, savedEntities.Count);
    }

    [Fact]
    public async Task Create_EmptyCollection_ReturnsEmptyResult()
    {
        // Arrange
        var sut = CreateSut();
        var emptyConfigs = Array.Empty<GlobalFileCollectionConfig>();

        // Act
        var result = await sut.Create(emptyConfigs);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Update_ExistingGlobalFileCollectionConfig_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var configEntity = CreateValidGlobalFileCollectionConfigEntity();
        
        await SeedGlobalFileCollectionConfigsAsync(configEntity);

        var updatedConfig = new GlobalFileCollectionConfig
        {
            Id = configEntity.Id,
            AutoFaissSync = !configEntity.AutoFaissSync, // Toggle the value
            UserId = configEntity.UserId
        };

        // Act
        var result = await sut.Update([updatedConfig]);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Single(result.Data);
        Assert.Equal(!configEntity.AutoFaissSync, result.Data.First().AutoFaissSync);
        Assert.Equal(configEntity.UserId, result.Data.First().UserId);
        
        // Verify the update in database
        var updatedEntity = await GetGlobalFileCollectionConfigFromDbAsync(configEntity.Id);
        Assert.NotNull(updatedEntity);
        Assert.Equal(!configEntity.AutoFaissSync, updatedEntity.AutoFaissSync);
        Assert.Equal(configEntity.UserId, updatedEntity.UserId);
    }

    [Fact]
    public async Task Update_NonExistentGlobalFileCollectionConfig_ThrowsException()
    {
        // Arrange
        var sut = CreateSut();
        var nonExistentConfig = CreateValidGlobalFileCollectionConfig();

        // Act & Assert - Updating non-existent entity throws exception
        await Assert.ThrowsAnyAsync<Exception>(() => sut.Update([nonExistentConfig]));
    }

    [Fact]
    public async Task Delete_ExistingGlobalFileCollectionConfigs_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var configEntities = new[]
        {
            CreateValidGlobalFileCollectionConfigEntity(),
            CreateValidGlobalFileCollectionConfigEntity()
        };
        
        await SeedGlobalFileCollectionConfigsAsync(configEntities);

        var configsToDelete = configEntities.Select(e => e.ToModel()).ToArray();

        // Act
        var result = await sut.Delete(configsToDelete);

        // Assert
        Assert.Equal(configsToDelete, result.Data);
        
        // Verify deletion in database
        var remainingEntities = await GetAllGlobalFileCollectionConfigsFromDbAsync();
        Assert.Empty(remainingEntities);
    }
    [Fact]
    public async Task Exists_ExistingGlobalFileCollectionConfig_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSut();
        var configEntity = CreateValidGlobalFileCollectionConfigEntity();
        
        await SeedGlobalFileCollectionConfigsAsync(configEntity);

        // Act
        var result = await sut.Exists(configEntity.Id);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task Exists_NonExistentGlobalFileCollectionConfig_ReturnsFalse()
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
    public async Task Exists_ByAutoFaissSync_ExistingConfig_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSut();
        var configEntity = CreateValidGlobalFileCollectionConfigEntity();
        
        await SeedGlobalFileCollectionConfigsAsync(configEntity);

        // Act
        var result = await sut.Exists(configEntity.AutoFaissSync, nameof(GlobalFileCollectionConfigEntity.AutoFaissSync));

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task Exists_ByAutoFaissSync_NonExistentConfig_ReturnsFalse()
    {
        // Arrange
        var sut = CreateSut();
        var configEntity = CreateValidGlobalFileCollectionConfigEntity();
        await SeedGlobalFileCollectionConfigsAsync(configEntity);
        var oppositeValue = !configEntity.AutoFaissSync;

        // Act
        var result = await sut.Exists(oppositeValue, nameof(GlobalFileCollectionConfigEntity.AutoFaissSync));

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.False(result.Data);
    }

    private async Task SeedGlobalFileCollectionConfigsAsync(params GlobalFileCollectionConfigEntity[] configs)
    {
        await using var context = new AiTrainerContext(_dbContextOptions);
        await context.GlobalFileCollectionConfigs.AddRangeAsync(configs);
        await context.SaveChangesAsync();
    }

    private async Task<GlobalFileCollectionConfigEntity?> GetGlobalFileCollectionConfigFromDbAsync(long id)
    {
        await using var context = new AiTrainerContext(_dbContextOptions);
        return await context.GlobalFileCollectionConfigs.FirstOrDefaultAsync(c => c.Id == id);
    }

    private async Task<List<GlobalFileCollectionConfigEntity>> GetAllGlobalFileCollectionConfigsFromDbAsync()
    {
        await using var context = new AiTrainerContext(_dbContextOptions);
        return await context.GlobalFileCollectionConfigs.ToListAsync();
    }

    public void Dispose()
    {
        // Nothing to dispose since we create new contexts for each operation
    }
}
