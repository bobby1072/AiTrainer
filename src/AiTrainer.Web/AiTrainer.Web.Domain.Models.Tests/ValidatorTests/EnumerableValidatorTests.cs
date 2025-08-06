using AiTrainer.Web.Domain.Models.Validators;
using AiTrainer.Web.TestBase;
using AutoFixture;
using FluentValidation;
using FluentValidation.TestHelper;

namespace AiTrainer.Web.Domain.Models.Tests.ValidatorTests;

public sealed class EnumerableValidatorTests : AiTrainerTestBase
{
    private sealed class TestItem
    {
        public required string Name { get; set; }
        public required int Value { get; set; }
    }

    private sealed class TestItemValidator : AbstractValidator<TestItem>
    {
        public TestItemValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Value).GreaterThan(0).WithMessage("Value must be greater than 0");
        }
    }

    private readonly EnumerableValidator<TestItem> _validator;

    public EnumerableValidatorTests()
    {
        var itemValidator = new TestItemValidator();
        _validator = new EnumerableValidator<TestItem>(itemValidator);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Items_Are_Valid()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new() { Name = "Item1", Value = 1 },
            new() { Name = "Item2", Value = 2 },
            new() { Name = "Item3", Value = 3 }
        };

        // Act & Assert
        var result = _validator.TestValidate(items);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Error_When_Item_Has_Invalid_Name()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new() { Name = "Valid Item", Value = 1 },
            new() { Name = "", Value = 2 }, // Invalid name
            new() { Name = "Another Valid Item", Value = 3 }
        };

        // Act & Assert
        var result = _validator.TestValidate(items);
        result.ShouldHaveValidationErrorFor("x[1].Name")
            .WithErrorMessage("Name is required");
    }

    [Fact]
    public void Should_Have_Error_When_Item_Has_Invalid_Value()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new() { Name = "Valid Item", Value = 1 },
            new() { Name = "Item with invalid value", Value = -1 }, // Invalid value
            new() { Name = "Another Valid Item", Value = 3 }
        };

        // Act & Assert
        var result = _validator.TestValidate(items);
        result.ShouldHaveValidationErrorFor("x[1].Value")
            .WithErrorMessage("Value must be greater than 0");
    }

    [Fact]
    public void Should_Have_Multiple_Errors_When_Multiple_Items_Are_Invalid()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new() { Name = "", Value = -1 }, // Both invalid
            new() { Name = "Valid Item", Value = 2 },
            new() { Name = "", Value = 0 } // Both invalid
        };

        // Act & Assert
        var result = _validator.TestValidate(items);
        result.ShouldHaveValidationErrorFor("x[0].Name").WithErrorMessage("Name is required");
        result.ShouldHaveValidationErrorFor("x[0].Value").WithErrorMessage("Value must be greater than 0");
        result.ShouldHaveValidationErrorFor("x[2].Name").WithErrorMessage("Name is required");
        result.ShouldHaveValidationErrorFor("x[2].Value").WithErrorMessage("Value must be greater than 0");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Collection_Is_Empty()
    {
        // Arrange
        var items = new List<TestItem>();

        // Act & Assert
        var result = _validator.TestValidate(items);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Collection_Has_Single_Valid_Item()
    {
        // Arrange
        var items = new List<TestItem>
        {
            new() { Name = "Single Item", Value = 42 }
        };

        // Act & Assert
        var result = _validator.TestValidate(items);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
