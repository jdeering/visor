using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
