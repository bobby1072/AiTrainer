using AutoFixture;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiTrainer.Web.Persistence.Tests.Repositories;

public sealed class UserRepositoryTests : IDisposable
{
    private readonly Fixture _fixture = new();
    private readonly Mock<ILogger<UserRepository>> _mockLogger = new();
    private readonly DbContextOptions<AiTrainerContext> _dbContextOptions;
    private readonly Mock<IDbContextFactory<AiTrainerContext>> _mockDbContextFactory = new();

    public UserRepositoryTests()
    {
        // Setup in-memory database
        _dbContextOptions = new DbContextOptionsBuilder<AiTrainerContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Setup mock factory to return new context instances
        _mockDbContextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult(new AiTrainerContext(_dbContextOptions)));
    }

    private UserRepository CreateSut() => new(_mockDbContextFactory.Object, _mockLogger.Object);

    private User CreateValidUser()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = _fixture.Create<string>() + "@example.com",
            Name = _fixture.Create<string>(),
            DateCreated = DateTime.UtcNow,
            DateModified = DateTime.UtcNow
        };
    }

    private UserEntity CreateValidUserEntity()
    {
        return new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = _fixture.Create<string>() + "@example.com",
            Name = _fixture.Create<string>(),
            DateCreated = DateTime.UtcNow,
            DateModified = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task GetCount_ReturnsCorrectCount()
    {
        // Arrange
        var sut = CreateSut();
        var users = new[]
        {
            CreateValidUserEntity(),
            CreateValidUserEntity(),
            CreateValidUserEntity()
        };
        
        await SeedUsersAsync(users);

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
    public async Task GetOne_ValidId_ReturnsUser()
    {
        // Arrange
        var sut = CreateSut();
        var userEntity = CreateValidUserEntity();
        
        await SeedUsersAsync(userEntity);

        // Act
        var result = await sut.GetOne(userEntity.Id);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Equal(userEntity.Id, result.Data.Id);
        Assert.Equal(userEntity.Email, result.Data.Email);
        Assert.Equal(userEntity.Name, result.Data.Name);
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
    public async Task GetOne_ByEmail_ReturnsUser()
    {
        // Arrange
        var sut = CreateSut();
        var userEntity = CreateValidUserEntity();
        
        await SeedUsersAsync(userEntity);

        // Act
        var result = await sut.GetOne(userEntity.Email, nameof(UserEntity.Email));

        // Assert
        Assert.NotNull(result.Data);
        Assert.Equal(userEntity.Email, result.Data.Email);
        Assert.Equal(userEntity.Id, result.Data.Id);
    }

    [Fact]
    public async Task GetOne_ByEmail_NonExistentEmail_ReturnsNull()
    {
        // Arrange
        var sut = CreateSut();
        var nonExistentEmail = "nonexistent@example.com";

        // Act
        var result = await sut.GetOne(nonExistentEmail, nameof(UserEntity.Email));

        // Assert
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetMany_ByIds_ReturnsMatchingUsers()
    {
        // Arrange
        var sut = CreateSut();
        var userEntities = new[]
        {
            CreateValidUserEntity(),
            CreateValidUserEntity(),
            CreateValidUserEntity()
        };
        
        await SeedUsersAsync(userEntities);

        var idsToFind = userEntities.Take(2).Select(u => u.Id).ToArray();

        // Act
        var result = await sut.GetMany(idsToFind);

        // Assert
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        Assert.All(result.Data, user => Assert.Contains(user.Id!.Value, idsToFind));
    }

    [Fact]
    public async Task GetMany_ByEmails_ReturnsMatchingUsers()
    {
        // Arrange
        var sut = CreateSut();
        var userEntities = new[]
        {
            CreateValidUserEntity(),
            CreateValidUserEntity(),
            CreateValidUserEntity()
        };
        
        await SeedUsersAsync(userEntities);

        var emailsToFind = userEntities.Take(2).Select(u => u.Email).ToArray();

        // Act
        var result = await sut.GetMany((IEnumerable<string>)emailsToFind, nameof(UserEntity.Email));

        // Assert
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        Assert.All(result.Data, user => Assert.Contains(user.Email, emailsToFind));
    }

    [Fact]
    public async Task Create_ValidUser_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var user = CreateValidUser();

        // Act
        var result = await sut.Create([user]);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Single(result.Data);
        Assert.Equal(user.Email, result.Data.First().Email);
        
        // Verify it was actually saved to database
        var savedEntity = await GetUserFromDbAsync(user.Email);
        Assert.NotNull(savedEntity);
        Assert.Equal(user.Email, savedEntity.Email);
    }

    [Fact]
    public async Task Create_MultipleUsers_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var users = new[]
        {
            CreateValidUser(),
            CreateValidUser(),
            CreateValidUser()
        };

        // Act
        var result = await sut.Create(users);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(3, result.Data.Count);
        
        // Verify all were saved to database
        var savedEntities = await GetAllUsersFromDbAsync();
        Assert.Equal(3, savedEntities.Count);
    }

    [Fact]
    public async Task Create_EmptyCollection_ReturnsEmptyResult()
    {
        // Arrange
        var sut = CreateSut();
        var emptyUsers = Array.Empty<User>();

        // Act
        var result = await sut.Create(emptyUsers);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task Update_ExistingUser_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var userEntity = CreateValidUserEntity();
        
        await SeedUsersAsync(userEntity);

        var updatedUser = new User
        {
            Id = userEntity.Id,
            Email = userEntity.Email,
            Name = "Updated Name",
            DateCreated = userEntity.DateCreated,
            DateModified = DateTime.UtcNow
        };

        // Act
        var result = await sut.Update([updatedUser]);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Single(result.Data);
        Assert.Equal("Updated Name", result.Data.First().Name);
        
        // Verify the update in database
        var updatedEntity = await GetUserFromDbAsync(userEntity.Id);
        Assert.NotNull(updatedEntity);
        Assert.Equal("Updated Name", updatedEntity.Name);
    }

    [Fact]
    public async Task Delete_ExistingUsers_ReturnsSuccess()
    {
        // Arrange
        var sut = CreateSut();
        var userEntities = new[]
        {
            CreateValidUserEntity(),
            CreateValidUserEntity()
        };
        
        await SeedUsersAsync(userEntities);

        var usersToDelete = userEntities.Select(e => e.ToModel()).ToArray();

        // Act
        var result = await sut.Delete(usersToDelete);

        // Assert
        Assert.Equal(usersToDelete, result.Data);
        
        // Verify deletion in database
        var remainingEntities = await GetAllUsersFromDbAsync();
        Assert.Empty(remainingEntities);
    }

    [Fact]
    public async Task Exists_ExistingUser_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSut();
        var userEntity = CreateValidUserEntity();
        
        await SeedUsersAsync(userEntity);

        // Act
        var result = await sut.Exists(userEntity.Id);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task Exists_NonExistentUser_ReturnsFalse()
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
    public async Task Exists_ByEmail_ExistingUser_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSut();
        var userEntity = CreateValidUserEntity();
        
        await SeedUsersAsync(userEntity);

        // Act
        var result = await sut.Exists(userEntity.Email, nameof(UserEntity.Email));

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task Exists_ByEmail_NonExistentUser_ReturnsFalse()
    {
        // Arrange
        var sut = CreateSut();
        var nonExistentEmail = "nonexistent@example.com";

        // Act
        var result = await sut.Exists(nonExistentEmail, nameof(UserEntity.Email));

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.False(result.Data);
    }

    private async Task SeedUsersAsync(params UserEntity[] users)
    {
        await using var context = new AiTrainerContext(_dbContextOptions);
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }

    private async Task<UserEntity?> GetUserFromDbAsync(Guid id)
    {
        await using var context = new AiTrainerContext(_dbContextOptions);
        return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    private async Task<UserEntity?> GetUserFromDbAsync(string email)
    {
        await using var context = new AiTrainerContext(_dbContextOptions);
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    private async Task<List<UserEntity>> GetAllUsersFromDbAsync()
    {
        await using var context = new AiTrainerContext(_dbContextOptions);
        return await context.Users.ToListAsync();
    }

    public void Dispose()
    {
        // Nothing to dispose since we create new contexts for each operation
    }
}
