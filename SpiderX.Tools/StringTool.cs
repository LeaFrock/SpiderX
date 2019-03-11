using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SpiderX.Tools
{
	public static class StringTool
	{
		#region Value

		private readonly static Regex _doubleRegex = new Regex(@"\d+(\.\d+)?", RegexOptions.None, TimeSpan.FromMilliseconds(500));

		public static int MatchInt(string text, int defaultValue = 0)
		{
			return TryMatchInt(text, out int result) ? result : defaultValue;
		}

		public static long MatchLong(string text, long defaultValue = 0)
		{
			return TryMatchLong(text, out long result) ? result : defaultValue;
		}

		public static double MatchDouble(string text, double defaultValue = 0)
		{
			return TryMatchDouble(text, out double result) ? result : defaultValue;
		}

		public static bool TryMatchInt(string text, out int value)
		{
			Match m = _doubleRegex.Match(text);
			if (!m.Success)
			{
				value = 0;
				return false;
			}
			return int.TryParse(m.Value, out value);
		}

		public static bool TryMatchLong(string text, out long value)
		{
			Match m = _doubleRegex.Match(text);
			if (!m.Success)
			{
				value = 0;
				return false;
			}
			return long.TryParse(m.Value, out value);
		}

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

		public static int[] MatchIntArray(string text, bool saveIfMatchFail = false)
		{
			var mc = _doubleRegex.Matches(text);
			if (mc.Count < 1)
			{
				return Array.Empty<int>();
			}
			List<int> result = new List<int>(mc.Count);
			for (int i = 0; i < mc.Count; i++)
			{
				if (mc[i].Success && int.TryParse(mc[i].Value, out int temp))
				{
					result.Add(temp);
				}
				else if (saveIfMatchFail)
				{
					result.Add(0);
				}
			}
			return result.ToArray();
		}

		#endregion Value

		#region IP

		private readonly static Regex _ipv4Regex = new Regex(@"(?=(\b|\D))(((\d{1,2})|(1\d{1,2})|(2[0-4]\d)|(25[0-5]))\.){3}((\d{1,2})|(1\d{1,2})|(2[0-4]\d)|(25[0-5]))(?=(\b|\D))", RegexOptions.None, TimeSpan.FromMilliseconds(500));
		private readonly static Regex _ipv6Regex = new Regex(@"^([\\da-fA-F]{1,4}:){7}([\\da-fA-F]{1,4})$", RegexOptions.None, TimeSpan.FromMilliseconds(500));

		#endregion IP
	}
}