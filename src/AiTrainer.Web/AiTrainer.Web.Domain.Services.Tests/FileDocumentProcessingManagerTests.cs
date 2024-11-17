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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using System.Text;

namespace AiTrainer.Web.Domain.Services.Tests
{
    public class FileDocumentProcessingManagerTests : DomainServiceTestBase
    {
        private readonly Mock<ILogger<FileDocumentProcessingManager>> _mockLogger  = new();
        private readonly Mock<IFileDocumentRepository> _mockFileDocumentRepository = new();
        private readonly Mock<IValidator<FileDocument>> _mockValidator = new();
        private readonly Mock<IFileCollectionRepository> _mockFileCollectionRepository = new();
        private readonly FileDocumentProcessingManager _fileDocumentProcessingManager;

        public FileDocumentProcessingManagerTests(): base()
        {
            _fileDocumentProcessingManager = new FileDocumentProcessingManager(
                _mockDomainServiceActionExecutor.Object,
                _mockapiRequestService,
                _mockLogger.Object,
                _mockFileDocumentRepository.Object,
                _mockValidator.Object,
                _mockFileCollectionRepository.Object
            );
        }

        [Fact]
        public async Task UploadFile_Should_Throw_If_No_Entities_Returned_From_Save()
        {
            //Arrange
            var currentUser = _fixture.Build<Models.User>().With(x => x.Id, Guid.NewGuid()).Create();
            var mockForm = FormFileUtils.CreateFromFile();
            var fileDocInput = _fixture
                .Build<FileDocumentSaveFormInput>()
                .With(x => x.CollectionId, (Guid?)null)
                .With(x => x.FileToCreate, mockForm)
                .Create();
            
            _mockDomainServiceActionExecutor.Setup(x => x.ExecuteAsync(It.IsAny<Expression<Func<IUserProcessingManager, Task<Models.User?>>>>(), default)).ReturnsAsync(currentUser);
            _mockValidator.Setup(x => x.ValidateAsync(It.IsAny<FileDocument>(), default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockFileDocumentRepository.Setup(x => x.Create(It.IsAny<IReadOnlyCollection<FileDocument>>())).ReturnsAsync(new Persistence.Models.DbSaveResult<FileDocument>());

            //Act
            var act = () => _fileDocumentProcessingManager.UploadFile(fileDocInput);

            //Assert
            await act.Should().ThrowAsync<ApiException>().WithMessage("Invalid file document");
        }

    }
}
