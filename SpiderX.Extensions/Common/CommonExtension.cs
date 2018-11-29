using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiderX.Extensions
{
	public static class CommonExtension
	{
		#region String

		public static bool ContainsAny(this string text, params string[] strs)
		{
			return Array.Exists(strs, s => text.Contains(s));
		}

		public static bool ContainsAny(this string text, IEnumerable<string> strs)
		{
			return strs.Any(s => text.Contains(s));
		}

		public static bool ContainsAll(this string text, params string[] strs)
		{
			return Array.TrueForAll(strs, s => text.Contains(s));
		}

		public static bool ContainsAll(this string text, IEnumerable<string> strs)
		{
			return strs.All(s => text.Contains(s));
		}

		#endregion String
	}
}