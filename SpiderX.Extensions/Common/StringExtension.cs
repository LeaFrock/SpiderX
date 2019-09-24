using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiderX.Extensions
{
	public static class StringExtension
	{
		public static bool ContainsAny(this string text, params string[] strs)
		{
			return Array.Exists(strs, s => text.Contains(s));
		}

		public static bool ContainsAny(this string text, StringComparison comparisonType, params string[] strs)
		{
			return Array.Exists(strs, s => text.Contains(s, comparisonType));
		}

		public static bool ContainsAny(this string text, IEnumerable<string> strs)
		{
			return strs.Any(s => text.Contains(s));
		}

		public static bool ContainsAny(this string text, StringComparison comparisonType, IEnumerable<string> strs)
		{
			return strs.Any(s => text.Contains(s, comparisonType));
		}

		public static bool ContainsAll(this string text, params string[] strs)
		{
			return Array.TrueForAll(strs, s => text.Contains(s));
		}

		public static bool ContainsAll(this string text, StringComparison comparisonType, params string[] strs)
		{
			return Array.TrueForAll(strs, s => text.Contains(s, comparisonType));
		}

		public static bool ContainsAll(this string text, IEnumerable<string> strs)
		{
			return strs.All(s => text.Contains(s));
		}

		public static bool ContainsAll(this string text, StringComparison comparisonType, IEnumerable<string> strs)
		{
			return strs.All(s => text.Contains(s, comparisonType));
		}

		public static bool StartsWithAny(this string text, params string[] strs)
		{
			return Array.Exists(strs, s => text.StartsWith(s));
		}

		public static bool StartsWithAny(this string text, StringComparison comparisonType, params string[] strs)
		{
			return Array.Exists(strs, s => text.StartsWith(s, comparisonType));
		}

		public static bool EndsWithAny(this string text, params string[] strs)
		{
			return Array.Exists(strs, s => text.EndsWith(s));
		}

		public static bool EndsWithAny(this string text, StringComparison comparisonType, params string[] strs)
		{
			return Array.Exists(strs, s => text.EndsWith(s, comparisonType));
		}
	}
}