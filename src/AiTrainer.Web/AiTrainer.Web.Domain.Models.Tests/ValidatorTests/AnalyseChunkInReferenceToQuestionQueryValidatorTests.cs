using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Models.Validators;
using AiTrainer.Web.TestBase;
using AutoFixture;
using FluentValidation.TestHelper;

namespace AiTrainer.Web.Domain.Models.Tests.ValidatorTests;

public class AnalyseChunkInReferenceToQuestionQueryValidatorTests : AiTrainerTestBase
{
    private readonly AnalyseChunkInReferenceToQuestionQueryValidator _validator;

    public AnalyseChunkInReferenceToQuestionQueryValidatorTests()
    {
        _validator = new AnalyseChunkInReferenceToQuestionQueryValidator();
    }

    [Fact]
    public void Should_Have_Error_When_ChunkId_Is_Empty()
    {
        // Arrange
        var model = _fixture.Build<AnalyseDocumentChunkInReferenceToQuestionQueryInput>()
            .With(x => x.ChunkId, Guid.Empty)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ChunkId)
            .WithErrorMessage("ChunkId cannot be empty");
    }

    [Fact]
    public void Should_Not_Have_Error_When_ChunkId_Is_Valid()
    {
        // Arrange
        var model = _fixture.Build<AnalyseDocumentChunkInReferenceToQuestionQueryInput>()
            .With(x => x.ChunkId, Guid.NewGuid())
            .With(x => x.Question, "Valid question")
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.ChunkId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_Question_Is_Empty(string? question)
    {
        // Arrange
        var model = _fixture.Build<AnalyseDocumentChunkInReferenceToQuestionQueryInput>()
            .With(x => x.Question, question!)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Question)
            .WithErrorMessage("Question cannot be empty");
    }

    [Fact]
    public void Should_Have_Error_When_Question_Is_Too_Long()
    {
        // Arrange
        var longQuestion = new string('a', 10001); // 10001 characters
        var model = _fixture.Build<AnalyseDocumentChunkInReferenceToQuestionQueryInput>()
            .With(x => x.Question, longQuestion)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Question)
            .WithErrorMessage("Question cannot be empty");
    }

    [Theory]
    [InlineData("Valid question")]
    [InlineData("A")]
    [InlineData("What is the meaning of this document chunk?")]
    public void Should_Not_Have_Error_When_Question_Is_Valid(string question)
    {
        // Arrange
        var model = _fixture.Build<AnalyseDocumentChunkInReferenceToQuestionQueryInput>()
            .With(x => x.Question, question)
            .With(x => x.ChunkId, Guid.NewGuid())
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Question);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Question_Is_Exactly_10000_Characters()
    {
        // Arrange
        var maxLengthQuestion = new string('a', 10000); // Exactly 10000 characters
        var model = _fixture.Build<AnalyseDocumentChunkInReferenceToQuestionQueryInput>()
            .With(x => x.Question, maxLengthQuestion)
            .With(x => x.ChunkId, Guid.NewGuid())
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Question);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
    {
        // Arrange
        var model = _fixture.Build<AnalyseDocumentChunkInReferenceToQuestionQueryInput>()
            .With(x => x.ChunkId, Guid.NewGuid())
            .With(x => x.Question, "Please analyze this document chunk in relation to the following question")
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
