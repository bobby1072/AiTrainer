using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Validators;
using AiTrainer.Web.TestBase;
using AutoFixture;
using FluentValidation.TestHelper;

namespace AiTrainer.Web.Domain.Models.Tests.ValidatorTests;

public class SimilaritySearchInputValidatorTests : AiTrainerTestBase
{
    private readonly SimilaritySearchInputValidator _validator;

    public SimilaritySearchInputValidatorTests()
    {
        _validator = new SimilaritySearchInputValidator();
    }

    [Theory]
    [InlineData(21)]
    [InlineData(50)]
    [InlineData(100)]
    public void Should_Have_Error_When_DocumentsToReturn_Exceeds_Limit(int documentsToReturn)
    {
        // Arrange
        var model = _fixture.Build<SimilaritySearchInput>()
            .With(x => x.DocumentsToReturn, documentsToReturn)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DocumentsToReturn)
            .WithErrorMessage("You cannot return that many documents");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(20)]
    public void Should_Not_Have_Error_When_DocumentsToReturn_Is_Valid(int documentsToReturn)
    {
        // Arrange
        var model = _fixture.Build<SimilaritySearchInput>()
            .With(x => x.DocumentsToReturn, documentsToReturn)
            .With(x => x.Question, "Valid question")
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.DocumentsToReturn);
    }

    [Fact]
    public void Should_Have_Error_When_Question_Is_Too_Long()
    {
        // Arrange
        var longQuestion = new string('a', 501); // 501 characters
        var model = _fixture.Build<SimilaritySearchInput>()
            .With(x => x.Question, longQuestion)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Question)
            .WithErrorMessage("Your question is too long");
    }

    [Theory]
    [InlineData("Short question")]
    [InlineData("A")]
    public void Should_Not_Have_Error_When_Question_Is_Valid_Length(string question)
    {
        // Arrange
        var model = _fixture.Build<SimilaritySearchInput>()
            .With(x => x.Question, question)
            .With(x => x.DocumentsToReturn, 10)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Question);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Question_Is_Exactly_500_Characters()
    {
        // Arrange
        var maxLengthQuestion = new string('a', 500); // Exactly 500 characters
        var model = _fixture.Build<SimilaritySearchInput>()
            .With(x => x.Question, maxLengthQuestion)
            .With(x => x.DocumentsToReturn, 10)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Question);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
    {
        // Arrange
        var model = _fixture.Build<SimilaritySearchInput>()
            .With(x => x.Question, "What is the meaning of life?")
            .With(x => x.DocumentsToReturn, 15)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
