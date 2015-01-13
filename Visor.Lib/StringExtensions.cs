using System.Globalization;

namespace Visor.Extensions
{
    public static class StringExtensions
    {
        public static string Titleize(this string s)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s);
        }

        public static string[] Lines(this string s)
        {
            return s.Split('\n');
        }
    }
}