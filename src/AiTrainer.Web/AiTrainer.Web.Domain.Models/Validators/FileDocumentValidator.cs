using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace AiTrainer.Web.Domain.Models.Validators
{
    internal class FileDocumentValidator : BaseValidator<FileDocument>
    {
        public FileDocumentValidator()
        {
            RuleFor(x => x.FileType).Must(NotNullFileType).WithMessage(ValidatorConstants.InvalidFileType);
            RuleFor(x => x.FileName).Must(IsStringNotNullEmptyOrWhitespace).WithMessage(ValidatorConstants.InvalidFileName);
            RuleFor(x => x.FileName).Must(x => NotLongerThan(x, 150)).WithMessage(ValidatorConstants.InvalidFileName);
            RuleFor(x => x.FileData).NotEmpty().WithMessage(ValidatorConstants.InvalidFileData);
            
            RuleFor(x => x.FileData).Must(FileIsNotTooLarge).WithMessage(ValidatorConstants.InvalidFileData);
            
            RuleFor(x => x.DateCreated).NotEmpty().WithMessage(ValidatorConstants.InvalidDate);
            RuleFor(x => x.DateCreated).Must(IsDateNowOrInThePast).WithMessage(ValidatorConstants.InvalidDate);
        }
        private static bool NotNullFileType(FileTypeEnum fileType) => fileType == FileTypeEnum.Null ? false : true;

        private static bool FileIsNotTooLarge(byte[] fileData)
        {
            double sizeInMegabytes = fileData.Length / (1024.0 * 1024.0);
            return sizeInMegabytes <= 3;
        }
    }
}