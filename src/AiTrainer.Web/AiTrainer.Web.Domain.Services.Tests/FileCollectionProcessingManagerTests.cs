using System.Linq.Expressions;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Partials;
using AiTrainer.Web.Domain.Services.File.Concrete;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.TestBase.Utils;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiTrainer.Web.Domain.Services.Tests
{
    public class FileCollectionProcessingManagerTests : DomainServiceTestBase
    {
        private readonly Mock<IFileCollectionRepository> _mockRepository = new();
        private readonly Mock<ILogger<FileCollectionProcessingManager>> _mockLogger = new();
        private readonly Mock<IValidator<FileCollection>> _mockValidator = new();
        private readonly Mock<IFileDocumentRepository> _mockFileDocumentRepository = new();
        private readonly FileCollectionProcessingManager _fileCollectionManager;

        public FileCollectionProcessingManagerTests()
            : base()
        {
            _fileCollectionManager = new FileCollectionProcessingManager(
                MockDomainServiceActionExecutor.Object,
                MockApiRequestService,
                _mockRepository.Object,
                _mockLogger.Object,
                _mockValidator.Object,
                _mockFileDocumentRepository.Object
            );
        }

        [Fact]
        public async Task SaveFileCollection_Should_Stop_Unauthorized_Users_From_Saving()
        {
            //Arrange
            var fileCollectionInput = Fixture.Create<FileCollectionSaveInput>();
            MockDomainServiceActionExecutor
                .Setup(x =>
                    x.ExecuteAsync(
                        It.IsAny<Expression<Func<IUserProcessingManager, Task<Models.User?>>>>(),
                        default
                    )
                )
                .ReturnsAsync((Models.User?)null);

            //Act
            var act = () => _fileCollectionManager.SaveFileCollection(fileCollectionInput);

            //Assert
            await act.Should().ThrowAsync<ApiException>().WithMessage("Can't find user");
        }

        [Fact]
        public async Task SaveFileCollection_Should_Correctly_Build_And_Save_Collection_From_Input()
        {
            //Arrange
            IReadOnlyCollection<FileCollection> fileCollectionToSave = null;
            var currentUser = Fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var parentCollection = Fixture
                .Build<FileCollection>()
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.ParentId, (Guid?)null)
                .With(x => x.UserId, currentUser.Id)
                .Create();
            var fileCollectionInput = Fixture
                .Build<FileCollectionSaveInput>()
                .With(x => x.Id, (Guid?)null)
                .With(x => x.ParentId, parentCollection.Id)
                .Create();
            MockDomainServiceActionExecutor
                .Setup(x =>
                    x.ExecuteAsync(
                        It.IsAny<Expression<Func<IUserProcessingManager, Task<Models.User?>>>>(),
                        default
                    )
                )
                .ReturnsAsync(currentUser);
            _mockValidator
                .Setup(x =>
                    x.ValidateAsync(
                        It.Is<FileCollection>(x =>
                            x.Id == fileCollectionInput.Id
                            && x.CollectionName == fileCollectionInput.CollectionName
                        ),
                        default
                    )
                )
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockRepository
                .Setup(x =>
                    x.Create(
                        It.Is<IReadOnlyCollection<FileCollection>>(x =>
                            x.First().CollectionName == fileCollectionInput.CollectionName
                        )
                    )
                )
                .Callback((IReadOnlyCollection<FileCollection> x) => fileCollectionToSave = x)
                .ReturnsAsync(() => new DbSaveResult<FileCollection>(fileCollectionToSave));
            _mockRepository
                .Setup(x =>
                    x.GetOne(fileCollectionInput.ParentId, nameof(FileCollectionEntity.ParentId))
                )
                .ReturnsAsync(new DbGetOneResult<FileCollection>(parentCollection));
            //Act
            var result = await _fileCollectionManager.SaveFileCollection(fileCollectionInput);

            //Assert
            result.Should().NotBeNull();
            result.CollectionName.Should().Be(fileCollectionInput.CollectionName);
            result.ParentId.Should().Be(fileCollectionInput.ParentId);
            result.UserId.Should().Be((Guid)currentUser.Id!);

            MockDomainServiceActionExecutor.Verify(
                x =>
                    x.ExecuteAsync(
                        It.IsAny<Expression<Func<IUserProcessingManager, Task<Models.User?>>>>(),
                        default
                    ),
                Times.Once
            );
            _mockValidator.Verify(
                x =>
                    x.ValidateAsync(
                        It.Is<FileCollection>(x =>
                            x.Id == fileCollectionInput.Id
                            && x.CollectionName == fileCollectionInput.CollectionName
                        ),
                        default
                    ),
                Times.Once
            );
            _mockRepository.Verify(x => x.GetOne(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task SaveFileCollection_Should_Correctly_Build_And_Update_File()
        {
            //Arrange
            IReadOnlyCollection<FileCollection> fileCollectionToSave = null;
            var currentUser = Fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            
            var originalFileCollection = Fixture
                .Build<FileCollection>()
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.DateCreated, RandomUtils.DateInThePast())
                .With(x => x.DateModified, RandomUtils.DateInThePast())
                .With(x => x.UserId, currentUser.Id)
                .With(x => x.ParentId, (Guid?)null)
                .With(x => x.Id, Guid.NewGuid())
                .Create();

            var newFileCollectionInput = Fixture
                .Build<FileCollectionSaveInput>()
                .With(x => x.Id, originalFileCollection.Id)
                .With(x => x.DateCreated, originalFileCollection.DateCreated)
                .With(x => x.DateModified, originalFileCollection.DateModified)
                .With(x => x.ParentId, (Guid?)null)
                .Create();

            MockDomainServiceActionExecutor
                .Setup(x =>
                    x.ExecuteAsync(
                        It.IsAny<Expression<Func<IUserProcessingManager, Task<Models.User?>>>>(),
                        default
                    )
                )
                .ReturnsAsync(currentUser);
            _mockValidator
                .Setup(x =>
                    x.ValidateAsync(
                        It.Is<FileCollection>(x =>
                            x.Id == newFileCollectionInput.Id
                            && x.CollectionName == newFileCollectionInput.CollectionName
                        ),
                        default
                    )
                )
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockRepository
                .Setup(x =>
                    x.Update(
                        It.Is<IReadOnlyCollection<FileCollection>>(x =>
                            x.First().CollectionName == newFileCollectionInput.CollectionName
                        )
                    )
                )
                .Callback((IReadOnlyCollection<FileCollection> x) => fileCollectionToSave = x)
                .ReturnsAsync(() => new DbSaveResult<FileCollection>(fileCollectionToSave));
            _mockRepository
                .Setup(x => x.GetOne((Guid)newFileCollectionInput.Id!))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(originalFileCollection));

            //Act
            var result = await _fileCollectionManager.SaveFileCollection(newFileCollectionInput);

            //Assert
            result.Should().NotBeNull();
            result.CollectionName.Should().Be(newFileCollectionInput.CollectionName);
            result.ParentId.Should().Be(newFileCollectionInput.ParentId);
            result.UserId.Should().Be((Guid)currentUser.Id!);

            MockDomainServiceActionExecutor.Verify(
                x =>
                    x.ExecuteAsync(
                        It.IsAny<Expression<Func<IUserProcessingManager, Task<Models.User?>>>>(),
                        default
                    ),
                Times.Once
            );
            _mockValidator.Verify(
                x =>
                    x.ValidateAsync(
                        It.Is<FileCollection>(x =>
                            x.Id == newFileCollectionInput.Id
                            && x.CollectionName == newFileCollectionInput.CollectionName
                        ),
                        default
                    ),
                Times.Once
            );
            _mockRepository.Verify(
                x =>
                    x.Update(
                        It.Is<IReadOnlyCollection<FileCollection>>(x =>
                            x.First().CollectionName == newFileCollectionInput.CollectionName
                        )
                    ),
                Times.Once
            );
            _mockRepository.Verify(x => x.GetOne((Guid)newFileCollectionInput.Id!), Times.Once);
        }

        [Fact]
        public async Task SaveFileCollection_Should_Throw_When_New_Model_Changes_LockedProperty()
        {
            //Arrange
            IReadOnlyCollection<FileCollection> fileCollectionToSave = null;
            var currentUser = Fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var originalFileCollection = Fixture
                .Build<FileCollection>()
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.DateCreated, RandomUtils.DateInThePast())
                .With(x => x.DateModified, RandomUtils.DateInThePast())
                .With(x => x.UserId, currentUser.Id)
                .With(x => x.Id, Guid.NewGuid())
                .Create();

            var newFileCollectionInput = Fixture
                .Build<FileCollectionSaveInput>()
                .With(x => x.Id, originalFileCollection.Id)
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.DateCreated, originalFileCollection.DateCreated)
                .Create();

            MockDomainServiceActionExecutor
                .Setup(x =>
                    x.ExecuteAsync(
                        It.IsAny<Expression<Func<IUserProcessingManager, Task<Models.User?>>>>(),
                        default
                    )
                )
                .ReturnsAsync(currentUser);
            _mockValidator
                .Setup(x =>
                    x.ValidateAsync(
                        It.Is<FileCollection>(x =>
                            x.Id == newFileCollectionInput.Id
                            && x.CollectionName == newFileCollectionInput.CollectionName
                        ),
                        default
                    )
                )
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mockRepository
                .Setup(x =>
                    x.Update(
                        It.Is<IReadOnlyCollection<FileCollection>>(x =>
                            x.First().CollectionName == newFileCollectionInput.CollectionName
                        )
                    )
                )
                .Callback((IReadOnlyCollection<FileCollection> x) => fileCollectionToSave = x)
                .ReturnsAsync(() => new DbSaveResult<FileCollection>(fileCollectionToSave));
            _mockRepository
                .Setup(x => x.GetOne((Guid)newFileCollectionInput.Id!))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(originalFileCollection));

            //Act
            var act = () => _fileCollectionManager.SaveFileCollection(newFileCollectionInput);

            //Assert
            await act.Should().ThrowAsync<ApiException>().WithMessage("Cannot edit those fields");

            MockDomainServiceActionExecutor.Verify(
                x =>
                    x.ExecuteAsync(
                        It.IsAny<Expression<Func<IUserProcessingManager, Task<Models.User?>>>>(),
                        default
                    ),
                Times.Once
            );
            _mockValidator.Verify(
                x =>
                    x.ValidateAsync(
                        It.Is<FileCollection>(x =>
                            x.Id == newFileCollectionInput.Id
                            && x.CollectionName == newFileCollectionInput.CollectionName
                        ),
                        default
                    ),
                Times.Once
            );
            _mockRepository.Verify(
                x => x.Update(It.IsAny<IReadOnlyCollection<FileCollection>>()),
                Times.Never
            );
            _mockRepository.Verify(
                x => x.Create(It.IsAny<IReadOnlyCollection<FileCollection>>()),
                Times.Never
            );
            _mockRepository.Verify(x => x.GetOne((Guid)newFileCollectionInput.Id!), Times.Once);
        }

        [Fact]
        public async Task GetOneLayerFileDocPartialsAndCollections_Given_Null_Collection_Id_Should_Get_Top_Levels()
        {
            //Arrange
            var currentUser = Fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var foundSingleFileCollection = Fixture
                .Build<FileCollection>()
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.DateCreated, RandomUtils.DateInThePast())
                .With(x => x.DateModified, RandomUtils.DateInThePast())
                .With(x => x.UserId, currentUser.Id)
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.ParentId, (Guid?)null)
                .Create();

            var foundSingleFileDocument = Fixture
                .Build<FileDocumentPartial>()
                .With(x => x.DateCreated, RandomUtils.DateInThePast())
                .With(x => x.CollectionId, (Guid?)null)
                .Create();

            MockDomainServiceActionExecutor
                .Setup(x =>
                    x.ExecuteAsync(
                        It.IsAny<Expression<Func<IUserProcessingManager, Task<Models.User?>>>>(),
                        default
                    )
                )
                .ReturnsAsync(currentUser);
            _mockRepository
                .Setup(x => x.GetTopLevelCollectionsForUser((Guid)currentUser.Id!))
                .ReturnsAsync(new DbGetManyResult<FileCollection>([foundSingleFileCollection]));

            _mockFileDocumentRepository
                .Setup(x => x.GetTopLevelDocumentPartialsForUser((Guid)currentUser.Id!))
                .ReturnsAsync(new DbGetManyResult<FileDocumentPartial>([foundSingleFileDocument]));

            //Act
            await _fileCollectionManager.GetOneLayerFileDocPartialsAndCollections();

            //Assert
            _mockRepository.Verify(
                x => x.GetTopLevelCollectionsForUser((Guid)currentUser.Id!),
                Times.Once
            );

            _mockFileDocumentRepository.Verify(
                x => x.GetTopLevelDocumentPartialsForUser((Guid)currentUser.Id!),
                Times.Once
            );
        }

        [Fact]
        public async Task GetOneLayerFileDocPartialsAndCollections_Given_Collection_Id_Should_Get_Children()
        {
            //Arrange
            var currentUser = Fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var foundSingleFileCollection = Fixture
                .Build<FileCollection>()
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.DateCreated, RandomUtils.DateInThePast())
                .With(x => x.DateModified, RandomUtils.DateInThePast())
                .With(x => x.UserId, currentUser.Id)
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.ParentId, Guid.NewGuid())
                .Create();

            var foundSingleFileDocument = Fixture
                .Build<FileDocumentPartial>()
                .With(x => x.DateCreated, RandomUtils.DateInThePast())
                .With(x => x.CollectionId, Guid.NewGuid())
                .Create();

            MockDomainServiceActionExecutor
                .Setup(x =>
                    x.ExecuteAsync(
                        It.IsAny<Expression<Func<IUserProcessingManager, Task<Models.User?>>>>(),
                        default
                    )
                )
                .ReturnsAsync(currentUser);
            _mockRepository
                .Setup(x =>
                    x.GetManyCollectionsForUser(
                        (Guid)foundSingleFileDocument.CollectionId!,
                        (Guid)currentUser.Id!
                    )
                )
                .ReturnsAsync(new DbGetManyResult<FileCollection>([foundSingleFileCollection]));

            _mockFileDocumentRepository
                .Setup(x =>
                    x.GetManyDocumentPartialsByCollectionId(
                        (Guid)foundSingleFileDocument.CollectionId!,
                        (Guid)currentUser.Id!
                    )
                )
                .ReturnsAsync(new DbGetManyResult<FileDocumentPartial>([foundSingleFileDocument]));

            //Act
            await _fileCollectionManager.GetOneLayerFileDocPartialsAndCollections(
                foundSingleFileCollection.Id
            );

            //Assert
            _mockRepository.Verify(
                x =>
                    x.GetManyCollectionsForUser(
                        (Guid)foundSingleFileCollection.Id!,
                        (Guid)currentUser.Id!
                    ),
                Times.Once
            );

            _mockFileDocumentRepository.Verify(
                x =>
                    x.GetManyDocumentPartialsByCollectionId(
                        (Guid)foundSingleFileCollection.Id!,
                        (Guid)currentUser.Id!
                    ),
                Times.Once
            );
        }
    }
}
