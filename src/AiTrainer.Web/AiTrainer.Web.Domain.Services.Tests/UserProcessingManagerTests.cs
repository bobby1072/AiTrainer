using AiTrainer.Web.Domain.Services.Abstract;
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
        protected readonly Mock<IDomainServiceActionExecutor> _domainServiceActionExecutor = new();
        private readonly UserProcessingManager _userProcessingManager;
        public UserProcessingManagerTests(): base()
        {
            _userProcessingManager = new UserProcessingManager(
                _repo.Object,
                _userInfoClient.Object,
                _logger.Object,
                _userValidator.Object,
                _cachingService.Object,
                _mockHttpContextAccessor.Object,
                _domainServiceActionExecutor.Object
            );
        }

        [Fact]
        public async Task SaveAndCacheUser_Should_Cache_And_Return_User_If_Db_And_Info_Repsonse_Match()
        {
            //Arrange
            var mockedUser = _fixture.Create<Models.User>();

            var userInfoResp = new UserInfoResponse
            {
                Email = mockedUser.Email,
                Name = mockedUser.Name!
            };
            var accessToken = Faker.Lorem.GetFirstWord();
            _repo.Setup(x => x.GetOne(mockedUser.Email, nameof(UserEntity.Email))).ReturnsAsync(new DbGetOneResult<Models.User>(mockedUser));

            _userInfoClient.Setup(x => x.TryInvokeAsync(accessToken)).ReturnsAsync(userInfoResp);

            //Act
            var result = await _userProcessingManager.SaveAndCacheUser(accessToken);

            //Assert
            result.Should().Be(mockedUser);
        }
    }
}
