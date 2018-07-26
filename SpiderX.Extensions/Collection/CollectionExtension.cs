using System.Collections.Generic;

namespace SpiderX.Extensions
{
	public static class CollectionExtension
	{
		public static bool IsNullOrEmpty<T>(this ICollection<T> source)
		{
			return source == null || source.Count < 1;
		}
	}
}