using AiTrainer.Web.Domain.Models.Validators;
using AiTrainer.Web.TestBase;
using AutoFixture;
using FluentValidation.TestHelper;

namespace AiTrainer.Web.Domain.Models.Tests.ValidatorTests;

public sealed class FileCollectionValidatorTests : AiTrainerTestBase
{
    private readonly FileCollectionValidator _validator;

    public FileCollectionValidatorTests()
    {
        _validator = new FileCollectionValidator();
    }

    [Fact]
    public void Should_Have_Error_When_UserId_Is_Empty()
    {
        // Arrange
        var model = new FileCollection
        {
            UserId = Guid.Empty,
            CollectionName = "Valid Name",
            DateCreated = DateTime.UtcNow.AddDays(-1),
            DateModified = DateTime.UtcNow.AddDays(-1)
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage(ValidatorConstants.InvalidUserId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_UserId_Is_Valid()
    {
        // Arrange
        var model = new FileCollection
        {
            UserId = Guid.NewGuid(),
            CollectionName = "Valid Collection Name",
            CollectionDescription = "Valid Description",
            DateCreated = DateTime.UtcNow.AddDays(-1),
            DateModified = DateTime.UtcNow.AddDays(-1)
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.UserId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Should_Have_Error_When_CollectionName_Is_Invalid(string invalidName)
    {
        // Arrange
        var model = new FileCollection
        {
            UserId = Guid.NewGuid(),
            CollectionName = invalidName,
            DateCreated = DateTime.UtcNow.AddDays(-1),
            DateModified = DateTime.UtcNow.AddDays(-1)
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CollectionName)
            .WithErrorMessage(ValidatorConstants.InvalidName);
    }

    [Fact]
    public void Should_Have_Error_When_CollectionName_Is_Too_Long()
    {
        // Arrange
        var longName = new string('a', 101); // 101 characters
        var model = new FileCollection
        {
            UserId = Guid.NewGuid(),
            CollectionName = longName,
            DateCreated = DateTime.UtcNow.AddDays(-1),
            DateModified = DateTime.UtcNow.AddDays(-1)
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CollectionName)
            .WithErrorMessage(ValidatorConstants.InvalidName);
    }

    [Theory]
    [InlineData("Valid Name")]
    [InlineData("A")]
    [InlineData("This is exactly one hundred characters long and should be valid as it's at the boundary")]
    public void Should_Not_Have_Error_When_CollectionName_Is_Valid(string validName)
    {
        // Arrange
        var model = new FileCollection
        {
            UserId = Guid.NewGuid(),
            CollectionName = validName,
            CollectionDescription = "Valid Description",
            DateCreated = DateTime.UtcNow.AddDays(-1),
            DateModified = DateTime.UtcNow.AddDays(-1)
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.CollectionName);
    }

    [Fact]
    public void Should_Have_Error_When_CollectionDescription_Is_Too_Long()
    {
        // Arrange
        var longDescription = new string('a', 501); // 501 characters
        var model = new FileCollection
        {
            UserId = Guid.NewGuid(),
            CollectionName = "Valid Name",
            CollectionDescription = longDescription,
            DateCreated = DateTime.UtcNow.AddDays(-1),
            DateModified = DateTime.UtcNow.AddDays(-1)
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.CollectionDescription)
            .WithErrorMessage(ValidatorConstants.InvalidDescription);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Valid description")]
    public void Should_Not_Have_Error_When_CollectionDescription_Is_Valid(string? validDescription)
    {
        // Create a string that's exactly 500 characters
        var maxLengthDescription = new string('a', 500);
        
        // Arrange
        var model = new FileCollection
        {
            UserId = Guid.NewGuid(),
            CollectionName = "Valid Name",
            CollectionDescription = validDescription ?? maxLengthDescription,
            DateCreated = DateTime.UtcNow.AddDays(-1),
            DateModified = DateTime.UtcNow.AddDays(-1)
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.CollectionDescription);
    }

    [Fact]
    public void Should_Have_Error_When_DateCreated_Is_Empty()
    {
        // Arrange
        var model = new FileCollection
        {
            UserId = Guid.NewGuid(),
            CollectionName = "Valid Name",
            DateCreated = default(DateTime),
            DateModified = DateTime.UtcNow
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DateCreated)
            .WithErrorMessage(ValidatorConstants.InvalidDate);
    }

    [Fact]
    public void Should_Have_Error_When_DateCreated_Is_In_Future()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1);
        var model = new FileCollection
        {
            UserId = Guid.NewGuid(),
            CollectionName = "Valid Name",
            DateCreated = futureDate,
            DateModified = DateTime.UtcNow
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DateCreated)
            .WithErrorMessage(ValidatorConstants.InvalidDate);
    }

    [Fact]
    public void Should_Have_Error_When_DateModified_Is_Empty()
    {
        // Arrange
        var model = new FileCollection
        {
            UserId = Guid.NewGuid(),
            CollectionName = "Valid Name",
            DateCreated = DateTime.UtcNow.AddDays(-1),
            DateModified = default(DateTime)
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DateModified)
            .WithErrorMessage(ValidatorConstants.InvalidDate);
    }

    [Fact]
    public void Should_Have_Error_When_DateModified_Is_In_Future()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1);
        var model = new FileCollection
        {
            UserId = Guid.NewGuid(),
            CollectionName = "Valid Name",
            DateCreated = DateTime.UtcNow.AddDays(-1),
            DateModified = futureDate
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DateModified)
            .WithErrorMessage(ValidatorConstants.InvalidDate);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
    {
        // Arrange
        var model = new FileCollection
        {
            UserId = Guid.NewGuid(),
            CollectionName = "Valid Collection Name",
            CollectionDescription = "Valid Description",
            DateCreated = DateTime.UtcNow.AddDays(-1),
            DateModified = DateTime.UtcNow.AddDays(-1)
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Dates_Are_Current_Time()
    {
        // Arrange
        var currentTime = DateTime.UtcNow;
        var model = new FileCollection
        {
            UserId = Guid.NewGuid(),
            CollectionName = "Valid Collection Name",
            CollectionDescription = "Valid Description",
            DateCreated = currentTime,
            DateModified = currentTime
        };

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
