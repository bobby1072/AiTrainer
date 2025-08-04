using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Validators;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.TestBase;
using AutoFixture;
using FluentValidation.TestHelper;

namespace AiTrainer.Web.Domain.Models.Tests.ValidatorTests;

public class BaseChatGptFormattedQueryInputValidatorTests : AiTrainerTestBase
{
    private readonly BaseChatGptFormattedQueryInputValidator _validator;

    public BaseChatGptFormattedQueryInputValidatorTests()
    {
        _validator = new BaseChatGptFormattedQueryInputValidator();
    }

    // Create a concrete implementation for testing since BaseChatGptFormattedQueryInput is abstract
    private sealed record TestChatGptFormattedQueryInput : BaseChatGptFormattedQueryInput;

    [Theory]
    [InlineData(DefinedQueryFormatsEnum.AnalyseDocumentChunkInReferenceToQuestion)]
    [InlineData(DefinedQueryFormatsEnum.EditFileDocument)]
    public void Should_Not_Have_Error_When_DefinedQueryFormatsEnum_Is_Valid(DefinedQueryFormatsEnum validEnum)
    {
        // Arrange
        var model = new TestChatGptFormattedQueryInput
        {
            DefinedQueryFormatsEnum = (int)validEnum
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.DefinedQueryFormatsEnum);
    }

    [Fact]
    public void Should_Have_Error_When_DefinedQueryFormatsEnum_Is_Invalid()
    {
        // Arrange - using an invalid enum value (casting from int)
        var invalidEnum = 999;
        var model = new TestChatGptFormattedQueryInput
        {
            DefinedQueryFormatsEnum = invalidEnum
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DefinedQueryFormatsEnum)
            .WithErrorMessage("DefinedQueryFormatsEnum is not a valid value");
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
    {
        // Arrange
        var model = new TestChatGptFormattedQueryInput
        {
            DefinedQueryFormatsEnum = (int)DefinedQueryFormatsEnum.AnalyseDocumentChunkInReferenceToQuestion
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
