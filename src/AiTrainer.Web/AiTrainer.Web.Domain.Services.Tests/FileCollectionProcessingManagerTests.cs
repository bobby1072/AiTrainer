using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.File.Concrete;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AiTrainer.Web.Domain.Services.Tests
{
    public class FileCollectionProcessingManagerTests: DomainServiceTestBase
    {
        private readonly Mock<IFileCollectionRepository> _mockRepository = new();
        private readonly Mock<ILogger<FileCollectionProcessingManager>> _mockLogger = new();
        private readonly Mock<IValidator<FileCollection>> _mockValidator = new();
        private readonly FileCollectionProcessingManager _fileCollectionManager;
        private IReadOnlyCollection<FileCollection>? _fileCollectionToSave {  get; set; }
        public FileCollectionProcessingManagerTests(): base()
        {
            _fileCollectionManager = new FileCollectionProcessingManager(
                _mockDomainServiceActionExecutor.Object,
                _mockapiRequestService,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockValidator.Object
            );
        }

        [Fact]
        public async Task SaveFileCollection_Should_Stop_Unauthorized_Users_From_Saving()
        {
            //Arrange
            var fileCollectionInput = _fixture.Create<FileCollectionSaveInput>();
            _mockDomainServiceActionExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Expression<Func<IUserProcessingManager, Task<Models.User?>>>>(), default)).ReturnsAsync((Models.User?)null);

            //Act
            var act = () => _fileCollectionManager.SaveFileCollection(fileCollectionInput);

            //Assert
            await act.Should().ThrowAsync<ApiException>().WithMessage("Can't find user");
        }

        [Fact]
        public async Task SaveFileCollection_Should_Correcly_Build_And_Save_Collection_From_Input()
        {
            //Arrange
            IReadOnlyCollection<FileCollection> fileCollectionToSave = null;
            var currentUser = _fixture.Build<Models.User>().With(x => x.Id, Guid.NewGuid()).Create();
            var fileCollectionInput = _fixture.Build<FileCollectionSaveInput>().With(x => x.Id, (Guid?)null).Create();

            _mockDomainServiceActionExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Expression<Func<IUserProcessingManager, Task<Models.User?>>>>(), default)).ReturnsAsync(currentUser);
            _mockValidator.Setup(x => x.ValidateAsync(It.Is<FileCollection>(x =>  x.Id == fileCollectionInput.Id && x.CollectionName == fileCollectionInput.CollectionName), default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockRepository
                .Setup(x => x.Create(It.Is<IReadOnlyCollection<FileCollection>>(x => x.First().CollectionName== fileCollectionInput.CollectionName)))
                .Callback((IReadOnlyCollection<FileCollection> x) => fileCollectionToSave = x)
                .ReturnsAsync(() => new DbSaveResult<FileCollection>(fileCollectionToSave));

            //Act
            var result = await _fileCollectionManager.SaveFileCollection(fileCollectionInput);


            //Assert
            result.Should().NotBeNull();
            result.CollectionName.Should().Be(fileCollectionInput.CollectionName);
            result.ParentId.Should().Be(fileCollectionInput.ParentId);
            result.UserId.Should().Be((Guid)currentUser.Id!);
        }
    }
}
