using FluentValidation;
using System.Net.Mail;

namespace AiTrainer.Web.Domain.Models.Validators
{
    internal abstract class BaseValidator<T> : AbstractValidator<T> where T : class
    {
        protected static bool IsValidEmail(string? input)
        {
            try
            {
                if (string.IsNullOrEmpty(input)) return false;
                _ = new MailAddress(input);

                return true;
            }
            catch
            {
                return false;
            }
        }
        protected static bool IsStringNotNullEmptyOrWhitespace(string? input)
        {
            return !string.IsNullOrEmpty(input) && !input.All(char.IsWhiteSpace);
        }
        protected static bool IsDateNowOrInThePast(DateTime input)
        {

            return input.ToUniversalTime() <= DateTime.UtcNow;
        }
        protected static bool IsDateBeforeOrEqualTo(DateTime initialDate, DateTime dateToCheckAgainst)
        {
            return initialDate.Date <= dateToCheckAgainst;
        }
        protected static bool IsDateInFutureOrEqualTo(DateTime initialDate, DateTime dateToCheckAgainst)
        {
            return initialDate.Date >= dateToCheckAgainst;
        }
        protected static bool NotLongerThan(string? input, int amount)
        {
            if (string.IsNullOrEmpty(input) || amount < 0)
            {
                return true;
            }

            return input.Length <= amount;
        }
    }
}
