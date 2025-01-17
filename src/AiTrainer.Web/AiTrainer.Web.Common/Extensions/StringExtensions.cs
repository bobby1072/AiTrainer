using System.Net.Mail;
using System.Text.RegularExpressions;

namespace AiTrainer.Web.Common.Extensions
{
    public static class StringExtensions
    {
        public static Uri AppendPathToUrl(this string baseUrl, string path)
        {
            return new Uri(new Uri(baseUrl), path);
        }

        public static Uri AppendPathToUrl(this Uri baseUrl, string path)
        {
            return new Uri(baseUrl, path);
        }

        public static Uri AppendQueryToUrl(this string baseUrl, string query)
        {
            return new Uri($"{baseUrl}?{query}");
        }

        public static Uri AppendQueryToUrl(this Uri baseUrl, string query)
        {
            return new Uri($"{baseUrl.AbsoluteUri}?{query}");
        }

        public static string TrimBase64String(this string input)
        {
            string pattern = @"^data:image\/[^;]+;base64,";
            return Regex.Replace(input, pattern, string.Empty);
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
