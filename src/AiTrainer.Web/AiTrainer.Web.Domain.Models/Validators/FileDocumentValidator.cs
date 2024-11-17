using FluentValidation;

namespace AiTrainer.Web.Domain.Models.Validators
{
    internal class FileDocumentValidator : BaseValidator<FileDocument>
    {
        public FileDocumentValidator()
        {
            RuleFor(x => x.FileType).NotEmpty().WithMessage(ValidatorConstants.InvalidFileType);
            RuleFor(x => x.FileType).Must(NotNullFileType).WithMessage(ValidatorConstants.InvalidFileType);
            RuleFor(x => x.FileName).Must(IsStringNotNullEmptyOrWhitespace).WithMessage(ValidatorConstants.InvalidFileName);
            RuleFor(x => x.FileName).Must(x => NotLongerThan(x, 150)).WithMessage(ValidatorConstants.InvalidFileName);
            RuleFor(x => x.FileData).NotEmpty().WithMessage(ValidatorConstants.InvalidFileData);
            RuleFor(x => x.DateCreated).NotEmpty().WithMessage(ValidatorConstants.InvalidDate);
            RuleFor(x => x.DateCreated).Must(IsDateNowOrInThePast).WithMessage(ValidatorConstants.InvalidDate);
        }
        private static bool NotNullFileType(FileTypeEnum fileType) => fileType == FileTypeEnum.Null ? false : true;
    }
}