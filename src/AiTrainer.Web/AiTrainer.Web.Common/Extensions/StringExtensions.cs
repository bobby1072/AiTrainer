using System.Net.Mail;
using System.Text.RegularExpressions;

namespace AiTrainer.Web.Common.Extensions
{
    public static class StringExtensions
    {
        public static string TrimBase64String(this string input)
        {
            string pattern = @"^data:image\/[^;]+;base64,";
            return Regex.Replace(input, pattern, string.Empty);
        }

        public static string ToPascalCase(this string original)
        {
            Regex invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            Regex whiteSpace = new Regex(@"(?<=\s)");
            Regex startsWithLowerCaseChar = new Regex("^[a-z]");
            Regex firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
            Regex lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            Regex upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

            var pascalCase = invalidCharsRgx
                .Replace(whiteSpace.Replace(original, "_"), string.Empty)
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

            return string.Concat(pascalCase);
        }

        public static bool IsValidEmail(this string email)
        {
            try
            {
                MailAddress mailAddress = new MailAddress(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsJustNumbers(this string input)
        {
            return input.All(char.IsDigit);
        }

        public static bool IsJustLetters(this string input)
        {
            return input.All(char.IsLetter);
        }

        public static bool IsJustSpaces(this string input)
        {
            return input.All(char.IsWhiteSpace);
        }
    }
}
