using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Partials;
using AiTrainer.Web.Domain.Services.File.Concrete;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.TestBase;
using AiTrainer.Web.TestBase.Utils;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace AiTrainer.Web.Domain.Services.Tests
{
    public sealed class FileCollectionProcessingManagerTests : AiTrainerTestBase
    {
        private readonly Mock<IFileCollectionRepository> _mockRepository = new();
        private readonly Mock<ILogger<FileCollectionProcessingManager>> _mockLogger = new();
        private readonly Mock<IValidator<FileCollection>> _mockValidator = new();
        private readonly Mock<IFileDocumentRepository> _mockFileDocumentRepository = new();
        private readonly Mock<IRepository<SharedFileCollectionMemberEntity, Guid, SharedFileCollectionMember>> _mockSharedFileCollectionMemberRepository= new();
        private readonly Mock<IValidator<IEnumerable<SharedFileCollectionMember>>> _sharedFileCollectionMemberValidator = new();
        private readonly Mock<IRepository<UserEntity, Guid, Domain.Models.User>> _mockUserRepo = new();
        private readonly FileCollectionProcessingManager _fileCollectionManager;
        public FileCollectionProcessingManagerTests()
        {
            _fileCollectionManager = new FileCollectionProcessingManager(
                _mockRepository.Object,
                _mockLogger.Object,
                _mockValidator.Object,
                _mockFileDocumentRepository.Object,
                _mockSharedFileCollectionMemberRepository.Object,
                _sharedFileCollectionMemberValidator.Object,
                _mockUserRepo.Object,
                _mockHttpContextAccessor.Object
            );
        }

        [Fact]
        public async Task UnshareFileCollectionAsync_Should_Successfully_Unshare_File_Collection_For_Authorized_User()
        {
            //Arrange
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.Email, Faker.Internet.Email())
                .Create();
            var collection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.SharedFileCollectionMembers, [])
                .With(x => x.UserId, currentUser.Id)
                .Create();
            
            var foundSharedMember = _fixture
                .Build<SharedFileCollectionMember>()
                .With(x => x.UserId, Guid.NewGuid())
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.CollectionId, collection.Id)
                .With(x => x.ParentSharedMemberId, (Guid?)null)
                .Create();

            _mockSharedFileCollectionMemberRepository
                .Setup(x => x.GetOne((Guid)foundSharedMember.Id!))
                .ReturnsAsync(new DbGetOneResult<SharedFileCollectionMember>(foundSharedMember));

            _mockRepository
                .Setup(x => x.GetOne((Guid)collection.Id!))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(collection));

            _mockSharedFileCollectionMemberRepository
                .Setup(x =>
                    x.Delete(It.Is<IReadOnlyCollection<Guid>>(y => y.Single() == foundSharedMember.Id))
                )
                .ReturnsAsync(new DbDeleteResult<Guid>([(Guid)foundSharedMember.Id!]));

            //Act
            var result = await _fileCollectionManager.UnshareFileCollectionAsync(new RequiredGuidIdInput
            {
                Id = (Guid)foundSharedMember.Id!
            }, currentUser);
            
            //Assert
            Assert.Equal(foundSharedMember.Id, result);

            _mockSharedFileCollectionMemberRepository
                .Verify(x => x.GetOne((Guid)foundSharedMember.Id!), Times.Once);

            _mockRepository
                .Verify(x => x.GetOne((Guid)collection.Id!), Times.Once);

            _mockSharedFileCollectionMemberRepository
                .Verify(x =>
                    x.Delete(It.Is<IReadOnlyCollection<Guid>>(y => y.Single() == foundSharedMember.Id)),
                    Times.Once
                );
        }
        [Fact]
        public async Task UnshareFileCollectionAsync_Should_Throw_If_User_Unauthorized()
        {
            //Arrange
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.Email, Faker.Internet.Email())
                .Create();
            var collection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.SharedFileCollectionMembers, [])
                .With(x => x.UserId, Guid.NewGuid())
                .Create();
            
            var foundSharedMember = _fixture
                .Build<SharedFileCollectionMember>()
                .With(x => x.UserId, Guid.NewGuid())
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.CollectionId, collection.Id)
                .With(x => x.ParentSharedMemberId, (Guid?)null)
                .Create();

            _mockSharedFileCollectionMemberRepository
                .Setup(x => x.GetOne((Guid)foundSharedMember.Id!))
                .ReturnsAsync(new DbGetOneResult<SharedFileCollectionMember>(foundSharedMember));

            _mockRepository
                .Setup(x => x.GetOne((Guid)collection.Id!))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(collection));

            _mockSharedFileCollectionMemberRepository
                .Setup(x =>
                    x.Delete(It.Is<IReadOnlyCollection<Guid>>(y => y.Single() == foundSharedMember.Id))
                )
                .ReturnsAsync(new DbDeleteResult<Guid>([(Guid)foundSharedMember.Id!]));

            //Act
            var act = () => _fileCollectionManager.UnshareFileCollectionAsync(new RequiredGuidIdInput
            {
                Id = (Guid)foundSharedMember.Id!
            }, currentUser);
            
            //Assert
            var ex = await Assert.ThrowsAsync<ApiException>(act);
            Assert.Equal(ExceptionConstants.Unauthorized, ex.Message);

            _mockSharedFileCollectionMemberRepository
                .Verify(x => x.GetOne((Guid)foundSharedMember.Id!), Times.Once);

            _mockRepository
                .Verify(x => x.GetOne((Guid)collection.Id!), Times.Once);

            _mockSharedFileCollectionMemberRepository
                .Verify(x =>
                        x.Delete(It.IsAny<IReadOnlyCollection<Guid>>()),
                    Times.Never
                );
        }
        [Fact]
        public async Task UnshareFileCollectionAsync_Should_Throw_If_Member_Is_Not_Shared()
        {
            //Arrange
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.Email, Faker.Internet.Email())
                .Create();
            var collection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.SharedFileCollectionMembers, [])
                .With(x => x.UserId, currentUser.Id)
                .Create();
            
            var foundSharedMember = _fixture
                .Build<SharedFileCollectionMember>()
                .With(x => x.UserId, Guid.NewGuid())
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.CollectionId, collection.Id)
                .With(x => x.ParentSharedMemberId, (Guid?)null)
                .Create();

            _mockSharedFileCollectionMemberRepository
                .Setup(x => x.GetOne((Guid)foundSharedMember.Id!))
                .ReturnsAsync(new DbGetOneResult<SharedFileCollectionMember>());

            //Act
            var act = () => _fileCollectionManager.UnshareFileCollectionAsync(new RequiredGuidIdInput
            {
                Id = (Guid)foundSharedMember.Id!
            }, currentUser);
            
            //Assert
            var ex = await Assert.ThrowsAsync<ApiException>(act);
            Assert.Equal("Could not find file collection with that id", ex.Message);
            
            _mockSharedFileCollectionMemberRepository
                .Verify(x => x.GetOne((Guid)foundSharedMember.Id!), Times.Once);

            _mockRepository
                .Verify(x => x.GetOne(It.IsAny<Guid>()), Times.Never);

            _mockSharedFileCollectionMemberRepository
                .Verify(x =>
                    x.Delete(It.IsAny<IReadOnlyCollection<Guid>>()),
                    Times.Never
                );
        }
        [Fact]
        public async Task UnshareFileCollectionAsync_Should_Throw_If_Failed_To_Delete_For_Authorized_User()
        {
            //Arrange
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.Email, Faker.Internet.Email())
                .Create();
            var collection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.SharedFileCollectionMembers, [])
                .With(x => x.UserId, currentUser.Id)
                .Create();
            
            var foundSharedMember = _fixture
                .Build<SharedFileCollectionMember>()
                .With(x => x.UserId, Guid.NewGuid())
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.CollectionId, collection.Id)
                .With(x => x.ParentSharedMemberId, (Guid?)null)
                .Create();

            _mockSharedFileCollectionMemberRepository
                .Setup(x => x.GetOne((Guid)foundSharedMember.Id!))
                .ReturnsAsync(new DbGetOneResult<SharedFileCollectionMember>(foundSharedMember));

            _mockRepository
                .Setup(x => x.GetOne((Guid)collection.Id!))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(collection));

            _mockSharedFileCollectionMemberRepository
                .Setup(x =>
                    x.Delete(It.Is<IReadOnlyCollection<Guid>>(y => y.Single() == foundSharedMember.Id))
                )
                .ThrowsAsync(new Exception());

            //Act
            var act = () => _fileCollectionManager.UnshareFileCollectionAsync(new RequiredGuidIdInput
            {
                Id = (Guid)foundSharedMember.Id!
            }, currentUser);
            
            //Assert
            var ex = await Assert.ThrowsAsync<ApiException>(act);
            Assert.Equal("Could not delete document", ex.Message);

            _mockSharedFileCollectionMemberRepository
                .Verify(x => x.GetOne((Guid)foundSharedMember.Id!), Times.Once);

            _mockRepository
                .Verify(x => x.GetOne((Guid)collection.Id!), Times.Once);

            _mockSharedFileCollectionMemberRepository
                .Verify(x =>
                    x.Delete(It.Is<IReadOnlyCollection<Guid>>(y => y.Single() == foundSharedMember.Id)),
                    Times.Once
                );
        }
        [Fact]
        public async Task ShareFileCollectionAsync_Should_Successfully_Share_File_Collection_By_Email_For_Authorized_User()
        {
            //Arrange
            IReadOnlyCollection<SharedFileCollectionMember> callbackShareMems = [];
            var collectionId = Guid.NewGuid();
            var existingUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.Email, Faker.Internet.Email())
                .Create();
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.Email, Faker.Internet.Email())
                .Create();
            var collection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, collectionId)
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.SharedFileCollectionMembers, [])
                .With(x => x.UserId, currentUser.Id)
                .Create();
            var singleShareInput = _fixture
                .Build<SharedFileCollectionSingleMemberEmailSaveInput>()
                .With(x => x.Email, existingUser.Email)
                .Create();
            var sharedInput = _fixture
                .Build<SharedFileCollectionMemberSaveInput>()
                .With(x => x.CollectionId, collectionId)
                .With(x => x.MembersToShareTo,
                [
                    singleShareInput
                ])
                .Create();
            _mockUserRepo
                .Setup(x =>
                    x.GetMany<string>(new List<string>{existingUser.Email}, nameof(UserEntity.Email))
                )
                .ReturnsAsync(new DbGetManyResult<Models.User>([existingUser]));
            _mockRepository
                .Setup(x =>
                    x.GetCollectionWithChildren(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers))
                )
                .ReturnsAsync(new DbGetManyResult<FileCollection>([collection]));
            _sharedFileCollectionMemberValidator
                .Setup(x => x
                    .ValidateAsync(It.IsAny<IEnumerable<SharedFileCollectionMember>>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(new ValidationResult());
            _mockSharedFileCollectionMemberRepository
                .Setup(x =>
                    x.Create(
                        It.Is<IReadOnlyCollection<SharedFileCollectionMember>>(y =>
                            y.Single().CanCreateDocuments == singleShareInput.CanCreateDocuments &&
                            y.Single().CanDownloadDocuments == singleShareInput.CanDownloadDocuments &&
                            y.Single().CanRemoveDocuments == singleShareInput.CanRemoveDocuments &&
                            y.Single().CanViewDocuments == singleShareInput.CanViewDocuments &&
                            y.Single().CanSimilaritySearch == singleShareInput.CanSimilaritySearch)
                    )
                )
                .Callback((IReadOnlyCollection<SharedFileCollectionMember> x) => callbackShareMems = x)
                .ReturnsAsync(() => new DbSaveResult<SharedFileCollectionMember>(callbackShareMems));
            
            //Act
            var result = await _fileCollectionManager
                .ShareFileCollectionAsync(sharedInput, currentUser);

            //Assert
            Assert.Equal(callbackShareMems, result);

            _mockUserRepo
                .Verify(x =>
                    x.GetMany<string>(new List<string> { existingUser.Email }, nameof(UserEntity.Email)),
                    Times.Once
                );
            _mockRepository
                .Verify(x =>
                    x.GetCollectionWithChildren(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)),
                    Times.Once
                );
            _sharedFileCollectionMemberValidator
                .Verify(x => x
                    .ValidateAsync(It.IsAny<IEnumerable<SharedFileCollectionMember>>(), It.IsAny<CancellationToken>()),
                    Times.Once
                );
            _mockSharedFileCollectionMemberRepository
                .Verify(x =>
                    x.Create(
                        It.Is<IReadOnlyCollection<SharedFileCollectionMember>>(y =>
                            y.Single().CanCreateDocuments == singleShareInput.CanCreateDocuments &&
                            y.Single().CanDownloadDocuments == singleShareInput.CanDownloadDocuments &&
                            y.Single().CanRemoveDocuments == singleShareInput.CanRemoveDocuments &&
                            y.Single().CanViewDocuments == singleShareInput.CanViewDocuments &&
                            y.Single().CanSimilaritySearch == singleShareInput.CanSimilaritySearch)
                    ),
                    Times.Once
                );
        }
        [Fact]
        public async Task ShareFileCollectionAsync_Should_Throw_If_Email_Is_Not_Existing_User()
        {
            //Arrange
            IReadOnlyCollection<SharedFileCollectionMember> callbackShareMems = [];
            var collectionId = Guid.NewGuid();
            var existingUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.Email, Faker.Internet.Email())
                .Create();
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.Email, Faker.Internet.Email())
                .Create();
            var collection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, collectionId)
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.SharedFileCollectionMembers, [])
                .With(x => x.UserId, currentUser.Id)
                .Create();
            var singleShareInput = _fixture
                .Build<SharedFileCollectionSingleMemberEmailSaveInput>()
                .With(x => x.Email, existingUser.Email)
                .Create();
            var sharedInput = _fixture
                .Build<SharedFileCollectionMemberSaveInput>()
                .With(x => x.CollectionId, collectionId)
                .With(x => x.MembersToShareTo,
                [
                    singleShareInput
                ])
                .Create();
            _mockUserRepo
                .Setup(x =>
                    x.GetMany<string>(new List<string>{existingUser.Email}, nameof(UserEntity.Email))
                )
                .ReturnsAsync(new DbGetManyResult<Models.User>([]));
            
            //Act
            var act = () => _fileCollectionManager
                .ShareFileCollectionAsync(sharedInput, currentUser);

            //Assert
            await Assert.ThrowsAsync<ApiException>(act);

            _mockUserRepo
                .Verify(x =>
                    x.GetMany<string>(new List<string> { existingUser.Email }, nameof(UserEntity.Email)),
                    Times.Once
                );
            _mockRepository
                .Verify(x =>
                    x.GetCollectionWithChildren(It.IsAny<Guid>(), nameof(FileCollectionEntity.SharedFileCollectionMembers)),
                    Times.Never
                );
            _sharedFileCollectionMemberValidator
                .Verify(x => x
                    .ValidateAsync(It.IsAny<IEnumerable<SharedFileCollectionMember>>(), It.IsAny<CancellationToken>()),
                    Times.Never
                );
            _mockSharedFileCollectionMemberRepository
                .Verify(x =>
                    x.Create(
                        It.IsAny<IReadOnlyCollection<SharedFileCollectionMember>>()
                    ),
                    Times.Never
                );
        }
        [Fact]
        public async Task ShareFileCollectionAsync_Should_Successfully_Share_File_Collection_By_UserId_For_Authoized_User()
        {
            //Arrange
            IReadOnlyCollection<SharedFileCollectionMember> callbackShareMems = [];
            var collectionId = Guid.NewGuid();
            var existingUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.Email, Faker.Internet.Email())
                .Create();
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.Email, Faker.Internet.Email())
                .Create();
            var collection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, collectionId)
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.SharedFileCollectionMembers, [])
                .With(x => x.UserId, currentUser.Id)
                .Create();
            var singleShareInput = _fixture
                .Build<SharedFileCollectionSingleMemberUserIdSaveInput>()
                .With(x => x.UserId, existingUser.Id)
                .Create();
            var sharedInput = _fixture
                .Build<SharedFileCollectionMemberSaveInput>()
                .With(x => x.CollectionId, collectionId)
                .With(x => x.MembersToShareTo,
                [
                    singleShareInput
                ])
                .Create();
            _mockRepository
                .Setup(x =>
                    x.GetCollectionWithChildren(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers))
                )
                .ReturnsAsync(new DbGetManyResult<FileCollection>([collection]));
            _sharedFileCollectionMemberValidator
                .Setup(x => x
                    .ValidateAsync(It.IsAny<IEnumerable<SharedFileCollectionMember>>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(new ValidationResult());
            _mockSharedFileCollectionMemberRepository
                .Setup(x =>
                    x.Create(
                        It.Is<IReadOnlyCollection<SharedFileCollectionMember>>(y =>
                            y.Single().CanCreateDocuments == singleShareInput.CanCreateDocuments &&
                            y.Single().CanDownloadDocuments == singleShareInput.CanDownloadDocuments &&
                            y.Single().CanRemoveDocuments == singleShareInput.CanRemoveDocuments &&
                            y.Single().CanViewDocuments == singleShareInput.CanViewDocuments &&
                            y.Single().CanSimilaritySearch == singleShareInput.CanSimilaritySearch)
                    )
                )
                .Callback((IReadOnlyCollection<SharedFileCollectionMember> x) => callbackShareMems = x)
                .ReturnsAsync(() => new DbSaveResult<SharedFileCollectionMember>(callbackShareMems));
            
            //Act
            var result = await _fileCollectionManager
                .ShareFileCollectionAsync(sharedInput, currentUser);

            //Assert
            Assert.Equal(callbackShareMems, result);

            _mockUserRepo
                .Verify(x =>
                    x.GetMany<string>(It.IsAny<IReadOnlyCollection<string>>(), nameof(UserEntity.Email)),
                    Times.Never
                );
            _mockRepository
                .Verify(x =>
                    x.GetCollectionWithChildren(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)),
                    Times.Once
                );
            _sharedFileCollectionMemberValidator
                .Verify(x => x
                    .ValidateAsync(It.IsAny<IEnumerable<SharedFileCollectionMember>>(), It.IsAny<CancellationToken>()),
                    Times.Once
                );
            _mockSharedFileCollectionMemberRepository
                .Verify(x =>
                    x.Create(
                        It.Is<IReadOnlyCollection<SharedFileCollectionMember>>(y =>
                            y.Single().CanCreateDocuments == singleShareInput.CanCreateDocuments &&
                            y.Single().CanDownloadDocuments == singleShareInput.CanDownloadDocuments &&
                            y.Single().CanRemoveDocuments == singleShareInput.CanRemoveDocuments &&
                            y.Single().CanViewDocuments == singleShareInput.CanViewDocuments &&
                            y.Single().CanSimilaritySearch == singleShareInput.CanSimilaritySearch)

                    ),
                    Times.Once
                );
        }
        [Fact]
        public async Task ShareFileCollectionAsync_Should_Successfully_Share_File_Collection_With_Children_By_UserId_For_Authoized_User()
        {
            //Arrange
            IReadOnlyCollection<SharedFileCollectionMember> callbackShareMems = [];
            var collectionId = Guid.NewGuid();
            var existingUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.Email, Faker.Internet.Email())
                .Create();
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.Email, Faker.Internet.Email())
                .Create();
            var collection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, collectionId)
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.SharedFileCollectionMembers, [])
                .With(x => x.UserId, currentUser.Id)
                .Create();
            var childCollection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.SharedFileCollectionMembers, [])
                .With(x => x.UserId, currentUser.Id)
                .With(x => x.ParentId, collectionId)
                .Create();
            var singleShareInput = _fixture
                .Build<SharedFileCollectionSingleMemberUserIdSaveInput>()
                .With(x => x.UserId, existingUser.Id)
                .Create();
            var sharedInput = _fixture
                .Build<SharedFileCollectionMemberSaveInput>()
                .With(x => x.CollectionId, collectionId)
                .With(x => x.MembersToShareTo,
                [
                    singleShareInput
                ])
                .Create();
            _mockRepository
                .Setup(x =>
                    x.GetCollectionWithChildren(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers))
                )
                .ReturnsAsync(new DbGetManyResult<FileCollection>([collection, childCollection]));
            _sharedFileCollectionMemberValidator
                .Setup(x => x
                    .ValidateAsync(It.IsAny<IEnumerable<SharedFileCollectionMember>>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(new ValidationResult());
            _mockSharedFileCollectionMemberRepository
                .Setup(x =>
                    x.Create(
                        It.Is<IReadOnlyCollection<SharedFileCollectionMember>>(y =>
                            y.Count == 2 &&
                            y.All(z => z.CollectionId == collectionId || z.CollectionId == childCollection.Id) &&
                            y.All(z => z.CanCreateDocuments == singleShareInput.CanCreateDocuments) &&
                            y.All(z => z.CanDownloadDocuments == singleShareInput.CanDownloadDocuments) &&
                            y.All(z => z.CanRemoveDocuments == singleShareInput.CanRemoveDocuments) &&
                            y.All(z => z.CanViewDocuments == singleShareInput.CanViewDocuments) &&
                            y.All(z => z.CanSimilaritySearch == singleShareInput.CanSimilaritySearch)
                        )
                    )
                )
                .Callback((IReadOnlyCollection<SharedFileCollectionMember> x) => callbackShareMems = x)
                .ReturnsAsync(() => new DbSaveResult<SharedFileCollectionMember>(callbackShareMems));
            
            //Act
            var result = await _fileCollectionManager
                .ShareFileCollectionAsync(sharedInput, currentUser);

            //Assert
            Assert.Equal(callbackShareMems, result);

            _mockUserRepo
                .Verify(x =>
                    x.GetMany<string>(It.IsAny<IReadOnlyCollection<string>>(), nameof(UserEntity.Email)),
                    Times.Never
                );
            _mockRepository
                .Verify(x =>
                    x.GetCollectionWithChildren(collectionId, nameof(FileCollectionEntity.SharedFileCollectionMembers)),
                    Times.Once
                );
            _sharedFileCollectionMemberValidator
                .Verify(x => x
                    .ValidateAsync(It.IsAny<IEnumerable<SharedFileCollectionMember>>(), It.IsAny<CancellationToken>()),
                    Times.Once
                );
            _mockSharedFileCollectionMemberRepository
                .Verify(x =>
                        x.Create(
                            It.Is<IReadOnlyCollection<SharedFileCollectionMember>>(y =>
                                y.Count == 2 &&
                                y.All(z => z.CollectionId == collectionId || z.CollectionId == childCollection.Id) &&
                                y.All(z => z.CanCreateDocuments == singleShareInput.CanCreateDocuments) &&
                                y.All(z => z.CanDownloadDocuments == singleShareInput.CanDownloadDocuments) &&
                                y.All(z => z.CanRemoveDocuments == singleShareInput.CanRemoveDocuments) &&
                                y.All(z => z.CanViewDocuments == singleShareInput.CanViewDocuments) &&
                                y.All(z => z.CanSimilaritySearch == singleShareInput.CanSimilaritySearch)
                            )
                    ),
                    Times.Once
                );
        }
        [Fact]
        public async Task GetFileCollectionWithContentsAsync_Should_Return_File_Collection_With_Contents_For_Authorized_User()
        {
            //Arrange
            var collectionId = Guid.NewGuid();
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var mockFileBytes = await TestFileUtils.CreateFileBytes();
            var collection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, collectionId)
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.SharedFileCollectionMembers, [])
                .With(x => x.Documents, _fixture
                    .Build<FileDocument>()
                    .With(x => x.UserId, currentUser.Id)
                    .With(x => x.FileData, mockFileBytes)
                    .With(x => x.FileType, FileTypeEnum.Text)
                    .With(x => x.CollectionId, collectionId)
                    .CreateMany()
                    .ToArray()
                )
                .With(x => x.UserId, currentUser.Id)
                .Create();

            _mockRepository.Setup(x => x.GetOne(
                    (Guid)collection.Id!,
                    nameof(FileCollectionEntity.Documents),
                    nameof(FileCollectionEntity.SharedFileCollectionMembers)))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(collection));
            
            //Act
            var result = await _fileCollectionManager
                .GetFileCollectionWithContentsAsync(collectionId, currentUser);
            
            //Assert
            Assert.NotNull(result);
            Assert.Equal(collection, result);


            _mockRepository.Verify(x => x.GetOne(
                (Guid)collection.Id!,
                nameof(FileCollectionEntity.Documents),
                nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once);
        }
        [Fact]
        public async Task GetFileCollectionWithContentsAsync_Should_Throw_For_Unauthorized_User()
        {
            //Arrange
            var collectionId = Guid.NewGuid();
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var mockFileBytes = await TestFileUtils.CreateFileBytes();
            var collection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, collectionId)
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.SharedFileCollectionMembers, [])
                .With(x => x.Documents, _fixture
                    .Build<FileDocument>()
                    .With(x => x.UserId, Guid.NewGuid())
                    .With(x => x.FileData, mockFileBytes)
                    .With(x => x.FileType, FileTypeEnum.Text)
                    .With(x => x.CollectionId, collectionId)
                    .CreateMany()
                    .ToArray()
                )
                .With(x => x.UserId, Guid.NewGuid())
                .Create();

            _mockRepository.Setup(x => x.GetOne(
                    (Guid)collection.Id!,
                    nameof(FileCollectionEntity.Documents),
                    nameof(FileCollectionEntity.SharedFileCollectionMembers)))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(collection));
            
            //Act
            var act = () => _fileCollectionManager
                .GetFileCollectionWithContentsAsync(collectionId, currentUser);
            
            //Assert
            var ex = await Assert.ThrowsAsync<ApiException>(act);
            Assert.Equal(ExceptionConstants.Unauthorized, ex.Message);


            _mockRepository.Verify(x => x.GetOne(
                (Guid)collection.Id!,
                nameof(FileCollectionEntity.Documents),
                nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once);
        }
        [Fact]
        public async Task GetFileCollectionWithContentsAsync_Should_Return_File_Collection_With_Contents_For_Shared_Member_With_Permissions()
        {
            //Arrange
            var collectionId = Guid.NewGuid();
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var mockFileBytes = await TestFileUtils.CreateFileBytes();
            var collection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, collectionId)
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.SharedFileCollectionMembers, [
                    _fixture
                        .Build<SharedFileCollectionMember>()
                        .With(x => x.CollectionId, collectionId)
                        .With(x => x.UserId, currentUser.Id)
                        .With(x => x.CanDownloadDocuments, true)
                        .Create()
                ])
                .With(x => x.Documents, _fixture
                    .Build<FileDocument>()
                    .With(x => x.UserId, Guid.NewGuid())
                    .With(x => x.FileData, mockFileBytes)
                    .With(x => x.FileType, FileTypeEnum.Text)
                    .With(x => x.CollectionId, collectionId)
                    .CreateMany()
                    .ToArray()
                )
                .With(x => x.UserId, Guid.NewGuid())
                .Create();

            _mockRepository.Setup(x => x.GetOne(
                    (Guid)collection.Id!,
                    nameof(FileCollectionEntity.Documents),
                    nameof(FileCollectionEntity.SharedFileCollectionMembers)))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(collection));
            
            //Act
            var result = await _fileCollectionManager
                .GetFileCollectionWithContentsAsync(collectionId, currentUser);
            
            //Assert
            Assert.NotNull(result);
            Assert.Equal(collection, result);


            _mockRepository.Verify(x => x.GetOne(
                (Guid)collection.Id!,
                nameof(FileCollectionEntity.Documents),
                nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once);
        }
        [Fact]
        public async Task GetFileCollectionWithContentsAsync_Should_Throw_For_Shared_Member_With_No_Permissions()
        {
            //Arrange
            var collectionId = Guid.NewGuid();
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var mockFileBytes = await TestFileUtils.CreateFileBytes();
            var collection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, collectionId)
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.SharedFileCollectionMembers, [
                    _fixture
                        .Build<SharedFileCollectionMember>()
                        .With(x => x.CollectionId, collectionId)
                        .With(x => x.UserId, currentUser.Id)
                        .With(x => x.CanDownloadDocuments, false)
                        .Create()
                ])
                .With(x => x.Documents, _fixture
                    .Build<FileDocument>()
                    .With(x => x.UserId, Guid.NewGuid())
                    .With(x => x.FileData, mockFileBytes)
                    .With(x => x.FileType, FileTypeEnum.Text)
                    .With(x => x.CollectionId, collectionId)
                    .CreateMany()
                    .ToArray()
                )
                .With(x => x.UserId, Guid.NewGuid())
                .Create();

            _mockRepository.Setup(x => x.GetOne(
                    (Guid)collection.Id!,
                    nameof(FileCollectionEntity.Documents),
                    nameof(FileCollectionEntity.SharedFileCollectionMembers)))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(collection));
            
            //Act
            var act = () => _fileCollectionManager
                .GetFileCollectionWithContentsAsync(collectionId, currentUser);
            
            //Assert
            var ex = await Assert.ThrowsAsync<ApiException>(act);
            Assert.Equal(ExceptionConstants.Unauthorized, ex.Message);

            _mockRepository.Verify(x => x.GetOne(
                (Guid)collection.Id!,
                nameof(FileCollectionEntity.Documents),
                nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once);
        }
        [Fact]
        public async Task SaveFileCollectionAsync_With_Parent_Id_And_Shared_Members_On_Parent_Should_Correctly_Build_And_Save_Collection_From_Input()
        {
            //Arrange
            FileCollection? fileCollectionToSave = null;
            var parentCollectionId = Guid.NewGuid();
            var sharedMemberId = Guid.NewGuid();
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var parentCollection = _fixture
                .Build<FileCollection>()
                .With(x => x.Id, parentCollectionId)
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.ParentId, (Guid?)null)
                .With(x => x.SharedFileCollectionMembers, [
                    _fixture
                        .Build<SharedFileCollectionMember>()
                        .With(x => x.CollectionId, parentCollectionId)
                        .With(x => x.UserId, sharedMemberId)
                        .Create()
                ])
                .With(x => x.UserId, currentUser.Id)
                .Create();
            var fileCollectionInput = _fixture
                .Build<FileCollectionSaveInput>()
                .With(x => x.Id, (Guid?)null)
                .With(x => x.ParentId, parentCollection.Id)
                .Create();

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
                .ReturnsAsync(new ValidationResult());
            _mockRepository
                .Setup(x =>
                    x.CreateWithSharedMembers(
                        It.Is<FileCollection>(y =>
                            y.CollectionName == fileCollectionInput.CollectionName
                        ),
                        It.Is<IReadOnlyCollection<SharedFileCollectionMember>>(
                            y => y.Count == 1 &&
                                 y.Single().UserId == sharedMemberId
                            )
                    )
                )
                .Callback((FileCollection x, IReadOnlyCollection<SharedFileCollectionMember> _) =>
                    fileCollectionToSave = x
                )
                .ReturnsAsync(() => new DbSaveResult<FileCollection>(fileCollectionToSave is not null ? [fileCollectionToSave]: []));
            _mockRepository
                .Setup(x =>
                    x.GetOne(It.Is<Guid>(x => x == (Guid)fileCollectionInput.ParentId!), nameof(FileCollectionEntity.SharedFileCollectionMembers))
                )
                .ReturnsAsync(new DbGetOneResult<FileCollection>(parentCollection));
            //Act
            var result = await _fileCollectionManager.SaveFileCollectionAsync(fileCollectionInput, currentUser);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(fileCollectionInput.CollectionName, result.CollectionName);
            Assert.Equal(fileCollectionInput.ParentId, result.ParentId);
            Assert.Equal((Guid)currentUser.Id!, result.UserId);

            _mockValidator.Verify(
                x =>
                    x.ValidateAsync(
                        It.Is<FileCollection>(y =>
                            y.Id == fileCollectionInput.Id
                            && y.CollectionName == fileCollectionInput.CollectionName
                        ),
                        default
                    ),
                Times.Once
            );
            _mockRepository
                .Verify(x =>
                    x.GetOne(It.Is<Guid>(x => x == (Guid)fileCollectionInput.ParentId!), nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once
                );
            _mockRepository
                .Verify(x =>
                    x.CreateWithSharedMembers(
                        It.Is<FileCollection>(y =>
                            y.CollectionName == fileCollectionInput.CollectionName
                        ),
                        It.Is<IReadOnlyCollection<SharedFileCollectionMember>>(
                            y => y.Count == 1 &&
                                 y.Single().UserId == sharedMemberId
                        )
                    ), Times.Once
                );
        }
        [Fact]
        public async Task SaveFileCollectionAsync_With_Parent_Id_And_No_Shared_Members_Should_Correctly_Build_And_Save_Collection_From_Input()
        {
            //Arrange
            FileCollection? fileCollectionToSave = null;
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var parentCollection = _fixture
                .Build<FileCollection>()
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.ParentId, (Guid?)null)
                .With(x => x.SharedFileCollectionMembers, [])
                .With(x => x.UserId, currentUser.Id)
                .Create();
            var fileCollectionInput = _fixture
                .Build<FileCollectionSaveInput>()
                .With(x => x.Id, (Guid?)null)
                .With(x => x.ParentId, parentCollection.Id)
                .Create();

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
                    x.CreateWithSharedMembers(
                        It.Is<FileCollection>(y =>
                            y.CollectionName == fileCollectionInput.CollectionName
                        ),
                        It.Is<IReadOnlyCollection<SharedFileCollectionMember>>(y => y.Count == 0)
                    )
                )
                .Callback((FileCollection x, IReadOnlyCollection<SharedFileCollectionMember> _) =>
                    fileCollectionToSave = x
                )
                .ReturnsAsync(() => new DbSaveResult<FileCollection>(fileCollectionToSave is not null ? [fileCollectionToSave]: []));
            _mockRepository
                .Setup(x =>
                    x.GetOne(It.Is<Guid>(x => x == (Guid)fileCollectionInput.ParentId!), nameof(FileCollectionEntity.SharedFileCollectionMembers))
                )
                .ReturnsAsync(new DbGetOneResult<FileCollection>(parentCollection));
            //Act
            var result = await _fileCollectionManager.SaveFileCollectionAsync(fileCollectionInput, currentUser);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(fileCollectionInput.CollectionName, result.CollectionName);
            Assert.Equal(fileCollectionInput.ParentId, result.ParentId);
            Assert.Equal((Guid)currentUser.Id!, result.UserId);

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
            _mockRepository
                .Verify(x =>
                    x.GetOne(It.Is<Guid>(x => x == (Guid)fileCollectionInput.ParentId!), nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once
                );
            _mockRepository
                .Verify(x =>
                    x.CreateWithSharedMembers(
                        It.Is<FileCollection>(y =>
                            y.CollectionName == fileCollectionInput.CollectionName
                        ),
                        It.Is<IReadOnlyCollection<SharedFileCollectionMember>>(y => y.Count == 0)
                    ), Times.Once
                );
        }
        [Fact]
        public async Task SaveFileCollectionAsync_With_Parent_Should_Throw_If_User_Not_Authorized()
        {
            //Arrange
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var parentCollection = _fixture
                .Build<FileCollection>()
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.ParentId, (Guid?)null)
                .With(x => x.SharedFileCollectionMembers, [])
                .With(x => x.UserId, Guid.NewGuid())
                .Create();
            var fileCollectionInput = _fixture
                .Build<FileCollectionSaveInput>()
                .With(x => x.Id, (Guid?)null)
                .With(x => x.ParentId, parentCollection.Id)
                .Create();

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
                    x.GetOne(It.Is<Guid>(y => y == (Guid)fileCollectionInput.ParentId!), nameof(FileCollectionEntity.SharedFileCollectionMembers))
                )
                .ReturnsAsync(new DbGetOneResult<FileCollection>(parentCollection));
            //Act
            var act = () => _fileCollectionManager.SaveFileCollectionAsync(fileCollectionInput, currentUser);

            //Assert
            var ex = await Assert.ThrowsAsync<ApiException>(act);
            Assert.Equal(ExceptionConstants.Unauthorized, ex.Message);

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
            _mockRepository
                .Verify(x =>
                    x.GetOne(It.Is<Guid>(x => x == (Guid)fileCollectionInput.ParentId!), nameof(FileCollectionEntity.SharedFileCollectionMembers)), Times.Once
                );
            _mockRepository
                .Verify(x =>
                    x.CreateWithSharedMembers(
                        It.IsAny<FileCollection>(),
                        It.IsAny<IReadOnlyCollection<SharedFileCollectionMember>>()
                    ), Times.Never
                );
        }
        [Fact]
        public async Task SaveFileCollectionAsync_Should_Correctly_Build_And_Update_File()
        {
            //Arrange
            IReadOnlyCollection<FileCollection>? fileCollectionToSave = null;
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            
            var originalFileCollection = _fixture
                .Build<FileCollection>()
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.DateCreated, RandomUtils.DateInThePast())
                .With(x => x.DateModified, RandomUtils.DateInThePast())
                .With(x => x.UserId, currentUser.Id)
                .With(x => x.ParentId, (Guid?)null)
                .With(x => x.Id, Guid.NewGuid())
                .Create();

            var newFileCollectionInput = _fixture
                .Build<FileCollectionSaveInput>()
                .With(x => x.Id, originalFileCollection.Id)
                .With(x => x.DateCreated, originalFileCollection.DateCreated)
                .With(x => x.DateModified, originalFileCollection.DateModified)
                .With(x => x.ParentId, (Guid?)null)
                .Create();


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
                        It.Is<IReadOnlyCollection<FileCollection>>(y =>
                            y.Single().CollectionName == newFileCollectionInput.CollectionName
                        )
                    )
                )
                .Callback((IReadOnlyCollection<FileCollection> x) => fileCollectionToSave = x)
                .ReturnsAsync(() => new DbSaveResult<FileCollection>(fileCollectionToSave));
            _mockRepository
                .Setup(x => x.GetOne((Guid)newFileCollectionInput.Id!))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(originalFileCollection));

            //Act
            var result = await _fileCollectionManager.SaveFileCollectionAsync(newFileCollectionInput, currentUser);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(newFileCollectionInput.CollectionName, result.CollectionName);
            Assert.Equal(newFileCollectionInput.ParentId, result.ParentId);
            Assert.Equal((Guid)currentUser.Id!, result.UserId);


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
                        It.Is<IReadOnlyCollection<FileCollection>>(y =>
                            y.Single().CollectionName == newFileCollectionInput.CollectionName
                        )
                    ),
                Times.Once
            );
            _mockRepository.Verify(x => x.GetOne((Guid)newFileCollectionInput.Id!), Times.Once);
        }

        [Fact]
        public async Task SaveFileCollectionAsync_Should_Throw_When_New_Model_Changes_LockedProperty()
        {
            //Arrange
            IReadOnlyCollection<FileCollection>?  fileCollectionToSave = null;
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var originalFileCollection = _fixture
                .Build<FileCollection>()
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.DateCreated, RandomUtils.DateInThePast())
                .With(x => x.DateModified, RandomUtils.DateInThePast())
                .With(x => x.UserId, currentUser.Id)
                .With(x => x.Id, Guid.NewGuid())
                .Create();

            var newFileCollectionInput = _fixture
                .Build<FileCollectionSaveInput>()
                .With(x => x.Id, originalFileCollection.Id)
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.DateCreated, originalFileCollection.DateCreated)
                .Create();


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
                        It.Is<IReadOnlyCollection<FileCollection>>(y =>
                            y.Single().CollectionName == newFileCollectionInput.CollectionName
                        )
                    )
                )
                .Callback((IReadOnlyCollection<FileCollection> x) => fileCollectionToSave = x)
                .ReturnsAsync(() => new DbSaveResult<FileCollection>(fileCollectionToSave));
            _mockRepository
                .Setup(x => x.GetOne((Guid)newFileCollectionInput.Id!))
                .ReturnsAsync(new DbGetOneResult<FileCollection>(originalFileCollection));

            //Act
            var act = () => _fileCollectionManager.SaveFileCollectionAsync(newFileCollectionInput, currentUser);

            //Assert
            var thrownException = await Assert.ThrowsAsync<ApiException>(act);
            Assert.Equal("Cannot edit those fields", thrownException.Message);


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
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();

            var foundSingleFileDocument = _fixture
                .Build<FileDocumentPartial>()
                .With(x => x.DateCreated, RandomUtils.DateInThePast())
                .With(x => x.CollectionId, (Guid?)null)
                .With(x => x.UserId, currentUser.Id)
                .Create();
            

            _mockRepository
                .Setup(x => x.GetTopLevelCollectionsForUser((Guid)currentUser.Id!))
                .ReturnsAsync(new DbGetManyResult<FileCollection>([]));

            _mockFileDocumentRepository
                .Setup(x => x.GetTopLevelDocumentPartialsForUser((Guid)currentUser.Id!))
                .ReturnsAsync(new DbGetManyResult<FileDocumentPartial>([foundSingleFileDocument]));

            //Act
            await _fileCollectionManager.GetOneLayerFileDocPartialsAndCollectionsAsync(currentUser);

            //Assert
            _mockRepository.Verify(
                x => x.GetTopLevelCollectionsForUser((Guid)currentUser.Id!),
                Times.Once
            );

            _mockFileDocumentRepository.Verify(
                x => x.GetManyDocumentPartialsByCollectionIdAndUserId((Guid)currentUser.Id!, null,nameof(FileDocumentEntity.MetaData)),
                Times.Once
            );
        }

        [Fact]
        public async Task GetOneLayerFileDocPartialsAndCollections_Given_Collection_Id_Should_Get_Children()
        {
            //Arrange
            var currentUser = _fixture
                .Build<Models.User>()
                .With(x => x.Id, Guid.NewGuid())
                .Create();
            var foundSingleFileCollection = _fixture
                .Build<FileCollection>()
                .With(x => x.FaissStore, (FileCollectionFaiss?)null)
                .With(x => x.DateCreated, RandomUtils.DateInThePast())
                .With(x => x.DateModified, RandomUtils.DateInThePast())
                .With(x => x.UserId, currentUser.Id)
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.ParentId, Guid.NewGuid())
                .Create();

            var foundSingleFileDocument = _fixture
                .Build<FileDocumentPartial>()
                .With(x => x.DateCreated, RandomUtils.DateInThePast())
                .With(x => x.CollectionId, foundSingleFileCollection.Id)
                .With(x => x.UserId, currentUser.Id)
                .Create();
            

            
            _mockRepository
                .Setup(x =>
                    x.GetManyCollectionsForUserIncludingSelf(
                        (Guid)foundSingleFileCollection.Id!,
                        (Guid)currentUser.Id!
                    )
                )
                .ReturnsAsync(new DbGetManyResult<FileCollection>([foundSingleFileCollection]));

            _mockFileDocumentRepository
                .Setup(x =>
                    x.GetManyDocumentPartialsByCollectionIdAndUserId(
                        (Guid)currentUser.Id!,
                        (Guid)foundSingleFileDocument.CollectionId!
                    )
                )
                .ReturnsAsync(new DbGetManyResult<FileDocumentPartial>([foundSingleFileDocument]));

            //Act
            await _fileCollectionManager.GetOneLayerFileDocPartialsAndCollectionsAsync(
                currentUser, foundSingleFileCollection.Id
            );

            //Assert
            _mockRepository.Verify(
                x =>
                    x.GetManyCollectionsForUserIncludingSelf(
                        (Guid)foundSingleFileCollection.Id!,
                        (Guid)currentUser.Id!
                    ),
                Times.Once
            );

            _mockFileDocumentRepository.Verify(
                x =>
                    x.GetManyDocumentPartialsByCollectionId(
                        (Guid)foundSingleFileCollection.Id!,
                        nameof(FileDocumentEntity.MetaData)
                    ),
                Times.Once
            );
        }
    }
}
