using AiTrainer.Web.Domain.Models.Validators;
using AiTrainer.Web.TestBase;
using AutoFixture;
using FluentValidation.TestHelper;

namespace AiTrainer.Web.Domain.Models.Tests.ValidatorTests;

public class UserValidatorTests : AiTrainerTestBase
{
    private readonly UserValidator _validator;

    public UserValidatorTests()
    {
        _validator = new UserValidator();
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("test+tag@gmail.com")]
    public void Should_Not_Have_Error_When_Email_Is_Valid(string validEmail)
    {
        // Arrange
        var model = _fixture.Build<User>()
            .With(x => x.Email, validEmail)
            .With(x => x.DateCreated, DateTime.UtcNow.AddDays(-1))
            .With(x => x.DateModified, DateTime.UtcNow.AddDays(-1))
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@domain.com")]
    [InlineData("test@")]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_Email_Is_Invalid(string? invalidEmail)
    {
        // Arrange
        var model = _fixture.Build<User>()
            .With(x => x.Email, invalidEmail!)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email address");
    }

    [Fact]
    public void Should_Have_Error_When_DateCreated_Is_Empty()
    {
        // Arrange
        var model = _fixture.Build<User>()
            .With(x => x.DateCreated, default(DateTime))
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DateCreated)
            .WithErrorMessage("Invalid date");
    }

    [Fact]
    public void Should_Have_Error_When_DateCreated_Is_In_Future()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1);
        var model = _fixture.Build<User>()
            .With(x => x.DateCreated, futureDate)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DateCreated)
            .WithErrorMessage("Invalid date");
    }

    [Fact]
    public void Should_Have_Error_When_DateModified_Is_Empty()
    {
        // Arrange
        var model = _fixture.Build<User>()
            .With(x => x.DateModified, default(DateTime))
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DateModified)
            .WithErrorMessage("Invalid date");
    }

    [Fact]
    public void Should_Have_Error_When_DateModified_Is_In_Future()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1);
        var model = _fixture.Build<User>()
            .With(x => x.DateModified, futureDate)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DateModified)
            .WithErrorMessage("Invalid date");
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
    {
        // Arrange
        var model = _fixture.Build<User>()
            .With(x => x.Email, "test@example.com")
            .With(x => x.DateCreated, DateTime.UtcNow.AddDays(-1))
            .With(x => x.DateModified, DateTime.UtcNow.AddDays(-1))
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
