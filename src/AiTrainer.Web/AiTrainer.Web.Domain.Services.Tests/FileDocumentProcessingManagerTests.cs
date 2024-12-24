using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.File.Concrete;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.TestBase.Utils;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiTrainer.Web.Domain.Services.Tests
{
    public class FileDocumentProcessingManagerTests : DomainServiceTestBase
    {
        private readonly Mock<ILogger<FileDocumentProcessingManager>> _mockLogger = new();
        private readonly Mock<IFileDocumentRepository> _mockFileDocumentRepository = new();
        private readonly Mock<IValidator<FileDocument>> _mockValidator = new();
        private readonly Mock<IFileCollectionRepository> _mockFileCollectionRepository = new();
        private readonly FileDocumentProcessingManager _fileDocumentProcessingManager;
        private readonly Mock<IUserProcessingManager> _mockUserProcessingManager = new();
        public FileDocumentProcessingManagerTests()
            : base()
        {
            _fileDocumentProcessingManager = new FileDocumentProcessingManager(
                _mockUserProcessingManager.Object,
                MockContextAccessor.Object,
                _mockLogger.Object,
                _mockFileDocumentRepository.Object,
                _mockValidator.Object,
                _mockFileCollectionRepository.Object
            );
            AddAccessTokenToRequestHeaders();
        }

        [Fact]
        public async Task UploadFile_Should_Throw_If_No_Entities_Returned_From_Save()
        {
            //Arrange
            var currentUser = Fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var mockForm = FormFileUtils.CreateFromFile();
            var fileDocInput = Fixture
                .Build<FileDocumentSaveFormInput>()
                .With(x => x.CollectionId, (Guid?)null)
                .With(x => x.FileToCreate, mockForm)
                .Create();
            _mockUserProcessingManager.Setup(x => x.TryGetUserFromCache(It.IsAny<string>())).ReturnsAsync(currentUser);

            _mockValidator
                .Setup(x => x.ValidateAsync(It.IsAny<FileDocument>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockFileDocumentRepository
                .Setup(x => x.Create(It.IsAny<IReadOnlyCollection<FileDocument>>()))
                .ReturnsAsync(new Persistence.Models.DbSaveResult<FileDocument>());

            //Act
            var act = () => _fileDocumentProcessingManager.UploadFileDocument(fileDocInput);

            //Assert
            await act.Should().ThrowAsync<ApiException>().WithMessage("Invalid file document");
        }
    }
}
