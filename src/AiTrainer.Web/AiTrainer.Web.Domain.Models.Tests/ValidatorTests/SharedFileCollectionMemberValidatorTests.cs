using AiTrainer.Web.Domain.Models.Validators;
using AiTrainer.Web.TestBase;
using AutoFixture;
using FluentValidation.TestHelper;

namespace AiTrainer.Web.Domain.Models.Tests.ValidatorTests;

public class SharedFileCollectionMemberValidatorTests : AiTrainerTestBase
{
    private readonly SharedFileCollectionMemberValidator _validator;

    public SharedFileCollectionMemberValidatorTests()
    {
        _validator = new SharedFileCollectionMemberValidator();
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        // Arrange
        var model = _fixture.Build<SharedFileCollectionMember>()
            .With(x => x.UserId, Guid.Empty)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("User id is required");
    }

    [Fact]
    public void Should_Not_Have_Error_When_UserId_Is_Valid()
    {
        // Arrange
        var model = _fixture.Build<SharedFileCollectionMember>()
            .With(x => x.UserId, Guid.NewGuid())
            .With(x => x.CollectionId, Guid.NewGuid())
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_Have_Error_When_CollectionId_Is_Empty()
    {
        // Arrange
        var model = _fixture.Build<SharedFileCollectionMember>()
            .With(x => x.CollectionId, Guid.Empty)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CollectionId)
            .WithErrorMessage("Collection id is required");
    }

    [Fact]
    public void Should_Not_Have_Error_When_CollectionId_Is_Valid()
    {
        // Arrange
        var model = _fixture.Build<SharedFileCollectionMember>()
            .With(x => x.UserId, Guid.NewGuid())
            .With(x => x.CollectionId, Guid.NewGuid())
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.CollectionId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
    {
        // Arrange
        var model = _fixture.Build<SharedFileCollectionMember>()
            .With(x => x.UserId, Guid.NewGuid())
            .With(x => x.CollectionId, Guid.NewGuid())
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
