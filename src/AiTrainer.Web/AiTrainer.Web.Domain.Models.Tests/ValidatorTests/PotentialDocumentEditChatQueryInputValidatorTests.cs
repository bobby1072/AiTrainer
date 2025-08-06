using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Validators;
using AiTrainer.Web.TestBase;
using AutoFixture;
using FluentValidation.TestHelper;

namespace AiTrainer.Web.Domain.Models.Tests.ValidatorTests;

public sealed class PotentialDocumentEditChatQueryInputValidatorTests : AiTrainerTestBase
{
    private readonly PotentialDocumentEditChatQueryInputValidator _validator;

    public PotentialDocumentEditChatQueryInputValidatorTests()
    {
        _validator = new PotentialDocumentEditChatQueryInputValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_ChangeRequest_Is_Empty(string? changeRequest)
    {
        // Arrange
        var model = _fixture.Build<PotentialDocumentEditChatRawQueryInput>()
            .With(x => x.ChangeRequest, changeRequest!)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ChangeRequest)
            .WithErrorMessage("Change request cannot be empty");
    }

    [Theory]
    [InlineData("Valid change request")]
    [InlineData("A")]
    [InlineData("Update the document with new information")]
    [InlineData("Please change the salary to £60,000")]
    public void Should_Not_Have_Error_When_ChangeRequest_Is_Valid(string changeRequest)
    {
        // Arrange
        var model = _fixture.Build<PotentialDocumentEditChatRawQueryInput>()
            .With(x => x.ChangeRequest, changeRequest)
            .With(x => x.FileDocumentId, Guid.NewGuid())
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.ChangeRequest);
    }

    [Fact]
    public void Should_Have_Error_When_FileDocumentId_Is_Empty()
    {
        // Arrange
        var model = _fixture.Build<PotentialDocumentEditChatRawQueryInput>()
            .With(x => x.FileDocumentId, Guid.Empty)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FileDocumentId)
            .WithErrorMessage("File document Id cannot be empty");
    }

    [Fact]
    public void Should_Not_Have_Error_When_FileDocumentId_Is_Valid()
    {
        // Arrange
        var model = _fixture.Build<PotentialDocumentEditChatRawQueryInput>()
            .With(x => x.FileDocumentId, Guid.NewGuid())
            .With(x => x.ChangeRequest, "Valid change request")
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.FileDocumentId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
    {
        // Arrange
        var model = _fixture.Build<PotentialDocumentEditChatRawQueryInput>()
            .With(x => x.ChangeRequest, "Please update this document with the latest information")
            .With(x => x.FileDocumentId, Guid.NewGuid())
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Multiple_Errors_When_Both_Properties_Are_Invalid()
    {
        // Arrange
        var model = _fixture.Build<PotentialDocumentEditChatRawQueryInput>()
            .With(x => x.ChangeRequest, "")
            .With(x => x.FileDocumentId, Guid.Empty)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ChangeRequest)
            .WithErrorMessage("Change request cannot be empty");
        result.ShouldHaveValidationErrorFor(x => x.FileDocumentId)
            .WithErrorMessage("File document Id cannot be empty");
    }
}
