using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SpiderX.Extensions
{
	public static class CollectionExtension
	{
		public static bool IsNullOrEmpty<T>(this ICollection<T> source)
		{
			return source == null || source.Count < 1;
		}

		#region Dictionary

		public static void AddRange<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> target, IEnumerable<KeyValuePair<TKey, TValue>> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException();
			}

			foreach (var item in source)
			{
				target.TryAdd(item.Key, item.Value);
			}
		}

		#endregion Dictionary
	}
}