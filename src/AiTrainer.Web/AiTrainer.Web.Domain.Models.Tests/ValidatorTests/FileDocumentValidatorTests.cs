using AiTrainer.Web.Domain.Models.Validators;
using AiTrainer.Web.TestBase;
using AutoFixture;
using FluentValidation.TestHelper;

namespace AiTrainer.Web.Domain.Models.Tests.ValidatorTests;

public sealed class FileDocumentValidatorTests : AiTrainerTestBase
{
    private readonly FileDocumentValidator _validator;

    public FileDocumentValidatorTests()
    {
        _validator = new FileDocumentValidator();
    }

    [Fact]
    public void Should_Have_Error_When_FileType_Is_Null()
    {
        // Arrange
        var model = _fixture.Build<FileDocument>()
            .With(x => x.FileType, FileTypeEnum.Null)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FileType)
            .WithErrorMessage(ValidatorConstants.InvalidFileType);
    }

    [Theory]
    [InlineData(FileTypeEnum.Text)]
    [InlineData(FileTypeEnum.Pdf)]
    public void Should_Not_Have_Error_When_FileType_Is_Valid(FileTypeEnum validFileType)
    {
        // Arrange
        var model = _fixture.Build<FileDocument>()
            .With(x => x.FileType, validFileType)
            .With(x => x.FileName, "ValidFileName.txt")
            .With(x => x.FileData, [1, 2, 3])
            .With(x => x.FileDescription, "Valid Description")
            .With(x => x.DateCreated, DateTime.UtcNow.AddDays(-1))
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.FileType);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Should_Have_Error_When_FileName_Is_Invalid(string? invalidFileName)
    {
        // Arrange
        var model = _fixture.Build<FileDocument>()
            .With(x => x.FileName, invalidFileName)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FileName)
            .WithErrorMessage(ValidatorConstants.InvalidFileName);
    }

    [Fact]
    public void Should_Have_Error_When_FileName_Is_Too_Long()
    {
        // Arrange
        var longFileName = new string('a', 151); // 151 characters
        var model = _fixture.Build<FileDocument>()
            .With(x => x.FileName, longFileName)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FileName)
            .WithErrorMessage(ValidatorConstants.InvalidFileName);
    }

    [Theory]
    [InlineData("ValidFileName.txt")]
    [InlineData("A")]
    [InlineData("This_is_exactly_one_hundred_and_fifty_characters_long_and_should_be_valid_as_its_at_the_boundary_limit_for_file_names_in_this_system")]
    public void Should_Not_Have_Error_When_FileName_Is_Valid(string validFileName)
    {
        // Arrange
        var model = _fixture.Build<FileDocument>()
            .With(x => x.FileName, validFileName)
            .With(x => x.FileType, FileTypeEnum.Text)
            .With(x => x.FileData, [1, 2, 3])
            .With(x => x.FileDescription, "Valid Description")
            .With(x => x.DateCreated, DateTime.UtcNow.AddDays(-1))
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.FileName);
    }

    [Fact]
    public void Should_Have_Error_When_FileData_Is_Empty()
    {
        // Arrange
        var model = _fixture.Build<FileDocument>()
            .With(x => x.FileData, Array.Empty<byte>())
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FileData)
            .WithErrorMessage(ValidatorConstants.InvalidFileData);
    }

    [Fact]
    public void Should_Have_Error_When_FileData_Is_Too_Large()
    {
        // Arrange
        var largeFileData = new byte[4 * 1024 * 1024]; // 4MB (over 3MB limit)
        var model = _fixture.Build<FileDocument>()
            .With(x => x.FileData, largeFileData)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FileData)
            .WithErrorMessage(ValidatorConstants.InvalidFileData);
    }

    [Fact]
    public void Should_Not_Have_Error_When_FileData_Is_Valid_Size()
    {
        // Arrange
        var validFileData = new byte[2 * 1024 * 1024]; // 2MB (under 3MB limit)
        var model = _fixture.Build<FileDocument>()
            .With(x => x.FileData, validFileData)
            .With(x => x.FileType, FileTypeEnum.Text)
            .With(x => x.FileName, "ValidFileName.txt")
            .With(x => x.FileDescription, "Valid Description")
            .With(x => x.DateCreated, DateTime.UtcNow.AddDays(-1))
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.FileData);
    }

    [Fact]
    public void Should_Have_Error_When_FileDescription_Is_Too_Long()
    {
        // Arrange
        var longDescription = new string('a', 501); // 501 characters
        var model = _fixture.Build<FileDocument>()
            .With(x => x.FileDescription, longDescription)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FileDescription)
            .WithErrorMessage(ValidatorConstants.InvalidDescription);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Valid description")]
    public void Should_Not_Have_Error_When_FileDescription_Is_Valid(string? validDescription)
    {
        // Create a string that's exactly 500 characters
        var maxLengthDescription = new string('a', 500);
        
        // Arrange
        var model = _fixture.Build<FileDocument>()
            .With(x => x.FileDescription, validDescription ?? maxLengthDescription)
            .With(x => x.FileType, FileTypeEnum.Text)
            .With(x => x.FileName, "ValidFileName.txt")
            .With(x => x.FileData, [1, 2, 3])
            .With(x => x.DateCreated, DateTime.UtcNow.AddDays(-1))
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.FileDescription);
    }

    [Fact]
    public void Should_Have_Error_When_DateCreated_Is_Empty()
    {
        // Arrange
        var model = _fixture.Build<FileDocument>()
            .With(x => x.DateCreated, default(DateTime))
            .Create();

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
        var model = _fixture.Build<FileDocument>()
            .With(x => x.DateCreated, futureDate)
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DateCreated)
            .WithErrorMessage(ValidatorConstants.InvalidDate);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
    {
        // Arrange
        var model = _fixture.Build<FileDocument>()
            .With(x => x.FileType, FileTypeEnum.Text)
            .With(x => x.FileName, "ValidFileName.txt")
            .With(x => x.FileData, [1, 2, 3, 4, 5])
            .With(x => x.FileDescription, "Valid Description")
            .With(x => x.DateCreated, DateTime.UtcNow.AddDays(-1))
            .Create();

        // Act & Assert
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
