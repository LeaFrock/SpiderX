using System;
using System.Text.RegularExpressions;

namespace SpiderX.Tools
{
    public static class StringTool
    {
        private readonly static Regex _doubleRegex = new Regex(@"\d+(\.\d+)?", RegexOptions.None, TimeSpan.FromMilliseconds(500));

        public static bool TryMatchDouble(string text, out double value)
        {
            Match m = _doubleRegex.Match(text);
            if (!m.Success)
            {
                value = 0d;
                return false;
            }
            return double.TryParse(m.Value, out value);
        }
    }
}