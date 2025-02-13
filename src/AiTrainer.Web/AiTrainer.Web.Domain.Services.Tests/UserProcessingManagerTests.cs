using AiTrainer.Web.Domain.Services.Abstract;
using AiTrainer.Web.Domain.Services.Concrete;
using AiTrainer.Web.Domain.Services.User.Concrete;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.UserInfoClient.Clients.Abstract;
using AiTrainer.Web.UserInfoClient.Models;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiTrainer.Web.Domain.Services.Tests
{
    public class UserProcessingManagerTests : DomainServiceTestBase
    {
        private readonly Mock<IRepository<UserEntity, Guid, Models.User>> _repo = new();
        private readonly Mock<IUserInfoClient> _userInfoClient = new();
        private readonly Mock<ILogger<UserProcessingManager>> _logger = new();
        private readonly Mock<IValidator<Models.User>> _userValidator = new();
        private readonly Mock<ICachingService> _cachingService = new();
        private readonly UserProcessingManager _userProcessingManager;

        public UserProcessingManagerTests()
        {
            _userProcessingManager = new UserProcessingManager(
                MockContextAccessor.Object,
                _repo.Object,
                _userInfoClient.Object,
                _logger.Object,
                _userValidator.Object,
                _cachingService.Object
            );
        }

        [Fact]
        public async Task SaveAndCacheUser_Should_Cache_And_Return_User_If_Db_And_Info_Response_Match()
        {
            //Arrange
            var mockedUser = Fixture.Create<Models.User>();

            var userInfoResponse = new UserInfoResponse
            {
                Email = mockedUser.Email,
                Name = mockedUser.Name!,
            };
            var accessToken = Faker.Lorem.GetFirstWord();
            _repo
                .Setup(x => x.GetOne(mockedUser.Email, nameof(UserEntity.Email)))
                .ReturnsAsync(new DbGetOneResult<Models.User>(mockedUser));

            _userInfoClient
                .Setup(x => x.TryInvokeAsync(accessToken))
                .ReturnsAsync(userInfoResponse);

            //Act
            var result = await _userProcessingManager.SaveAndCacheUser(accessToken);

            //Assert
            result.Should().Be(mockedUser);
            _cachingService.Verify(
                x =>
                    x.SetObject(
                        $"cacheUser-{accessToken}",
                        mockedUser,
                        CacheObjectTimeToLiveInSeconds.TenMinutes
                    ),
                Times.Once
            );
            _cachingService.Verify(
                x => x.TryGetObject<Models.User>($"cacheUser-{accessToken}"),
                Times.Once
            );
            _repo.Verify(x => x.GetOne(mockedUser.Email, nameof(UserEntity.Email)), Times.Once);
            _userInfoClient.Verify(x => x.TryInvokeAsync(accessToken), Times.Once);
        }

        [Fact]
        public async Task SaveAndCacheUser_Should_Create_And_Cache_New_User()
        {
            //Arrange
            var mockedUser = Fixture.Create<Models.User>();

            var userInfoResponse = new UserInfoResponse
            {
                Email = mockedUser.Email,
                Name = mockedUser.Name!,
            };
            var accessToken = Faker.Lorem.GetFirstWord();
            _repo
                .Setup(x => x.GetOne(mockedUser.Email, nameof(UserEntity.Email)))
                .ReturnsAsync(new DbGetOneResult<Models.User>());
            var validationResult = new FluentValidation.Results.ValidationResult();
            _userValidator
                .Setup(x => x.ValidateAsync(It.IsAny<Models.User>(), default))
                .ReturnsAsync(validationResult);
            _userInfoClient
                .Setup(x => x.TryInvokeAsync(accessToken))
                .ReturnsAsync(userInfoResponse);
            _repo
                .Setup(x => x.Create(It.IsAny<IReadOnlyCollection<Models.User>>()))
                .ReturnsAsync(new DbSaveResult<Models.User>([mockedUser]));

            //Act
            var result = await _userProcessingManager.SaveAndCacheUser(accessToken);

            //Assert
            result.Should().Be(mockedUser);
            _cachingService.Verify(
                x => x.TryGetObject<Models.User>($"cacheUser-{accessToken}"),
                Times.Once
            );
            _repo.Verify(x => x.GetOne(mockedUser.Email, nameof(UserEntity.Email)), Times.Once);
            _userInfoClient.Verify(x => x.TryInvokeAsync(accessToken), Times.Once);
            _repo.Verify(x => x.Create(It.IsAny<IReadOnlyCollection<Models.User>>()), Times.Once);
        }

        [Fact]
        public async Task SaveAndCacheUser_Should_Update_And_Cache_Old_User()
        {
            //Arrange
            var mockedUser = Fixture.Create<Models.User>();

            var userInfoResponse = new UserInfoResponse
            {
                Email = mockedUser.Email,
                Name = Faker.Name.FullName(),
            };
            var accessToken = Faker.Lorem.GetFirstWord();
            _repo
                .Setup(x => x.GetOne(mockedUser.Email, nameof(UserEntity.Email)))
                .ReturnsAsync(new DbGetOneResult<Models.User>(mockedUser));
            var validationResult = new FluentValidation.Results.ValidationResult();
            _userValidator
                .Setup(x => x.ValidateAsync(It.IsAny<Models.User>(), default))
                .ReturnsAsync(validationResult);
            _userInfoClient
                .Setup(x => x.TryInvokeAsync(accessToken))
                .ReturnsAsync(userInfoResponse);
            _repo
                .Setup(x => x.Update(It.IsAny<IReadOnlyCollection<Models.User>>()))
                .ReturnsAsync(new DbSaveResult<Models.User>([mockedUser]));

            //Act
            var result = await _userProcessingManager.SaveAndCacheUser(accessToken);

            //Assert
            result.Should().Be(mockedUser);
            _cachingService.Verify(
                x => x.TryGetObject<Models.User>($"cacheUser-{accessToken}"),
                Times.Once
            );
            _repo.Verify(x => x.GetOne(mockedUser.Email, nameof(UserEntity.Email)), Times.Once);
            _userInfoClient.Verify(x => x.TryInvokeAsync(accessToken), Times.Once);
            _repo.Verify(x => x.Update(It.IsAny<IReadOnlyCollection<Models.User>>()), Times.Once);
        }
    }
}
