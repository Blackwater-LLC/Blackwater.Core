using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Blackwater.Core.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string value) =>
            string.IsNullOrEmpty(value);

        public static bool IsNotNullOrEmpty(this string value) =>
            !string.IsNullOrEmpty(value);

        public static string ToTitleCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
        }

        public static string ExtractNumbers(this string value) =>
            new(value.Where(char.IsDigit).ToArray());

        public static int WordCount(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;
            return value.Split([' ', '\t', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static string Reverse(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return new string([.. value.Reverse()]);
        }

        public static string ToSlug(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            var normalized = value.ToLowerInvariant().Trim();
            normalized = Regex.Replace(normalized, @"\s+", "-");
            normalized = Regex.Replace(normalized, @"[^a-z0-9\-]", "");
            normalized = Regex.Replace(normalized, @"-+", "-");
            return normalized.Trim('-');
        }
    }
}
