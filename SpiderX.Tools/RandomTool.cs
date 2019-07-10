using System;
using System.Collections.Generic;

namespace SpiderX.Tools
{
	public static class RandomTool
	{
		private readonly static Random _randomEvent = new Random();

		public static int NextInt(int min, int max)
		{
			return _randomEvent.Next(min, max);
		}

		public static int NextIntSafely(int min, int max)
		{
			lock (_randomEvent)
			{
				return NextInt(min, max);
			}
		}

		public static long NextLong()
		{
			return (long)(_randomEvent.NextDouble() * long.MaxValue);
		}

		public static long NextLong(long min, long max)
		{
			return (long)(_randomEvent.NextDouble() * (max - min)) + min;
		}

		public static long NextLongSafely()
		{
			lock (_randomEvent)
			{
				return NextLong();
			}
		}

		public static long NextLongSafely(long min, long max)
		{
			lock (_randomEvent)
			{
				return NextLong(min, max);
			}
		}

		public static double NextDouble()
		{
			return _randomEvent.NextDouble();
		}

		public static double NextDouble(int min, int max)
		{
			return _randomEvent.NextDouble() * (max - min) + min;
		}

		public static double NextDoubleSafely()
		{
			lock (_randomEvent)
			{
				return NextDouble();
			}
		}

		public static double NextDoubleSafely(int min, int max)
		{
			lock (_randomEvent)
			{
				return NextDouble(min, max);
			}
		}

		public static void NextBytes(byte[] buffer)
		{
			_randomEvent.NextBytes(buffer);
		}

		public static void NextBytesSafely(byte[] buffer)
		{
			lock (_randomEvent)
			{
				_randomEvent.NextBytes(buffer);
			}
		}

		public static T NextElement<T>(IReadOnlyList<T> elements)
		{
			int index = _randomEvent.Next(0, elements.Count);
			return elements[index];
		}

		public static T NextElementSafely<T>(IReadOnlyList<T> elements)
		{
			lock (_randomEvent)
			{
				return NextElement(elements);
			}
		}

		public static T[] NextElements<T>(IReadOnlyList<T> elements, int length, bool allowRepeated = true)
		{
			int elementCount = elements.Count;
			var result = new T[length];
			if (allowRepeated)
			{
				for (int i = 0; i < length; i++)
				{
					result[i] = elements[_randomEvent.Next(0, elementCount)];
				}
			}
			else
			{
				if (length >= elementCount)
				{
					throw new ArgumentOutOfRangeException("The length must be less than the count of elements while allowing repeated.");
				}
				Dictionary<int, int> indexPatchDict = new Dictionary<int, int>(length / 2 + 1);//Give an estimate of capacity.
				for (int i = 0; i < length; i++)
				{
					int index = _randomEvent.Next(0, elementCount - i);
					if (indexPatchDict.TryGetValue(index, out int realIndex))
					{
						result[i] = elements[realIndex];
					}
					else
					{
						result[i] = elements[index];
					}
					if (index != elementCount - i - 1)
					{
						if (indexPatchDict.TryGetValue(elementCount - i - 1, out int otherRealIndex))
						{
							indexPatchDict[index] = otherRealIndex;
						}
						else
						{
							indexPatchDict[index] = elementCount - i - 1;
						}
					}
				}
			}
			return result;
		}

		public static T[] NextElementsSafely<T>(IReadOnlyList<T> elements, int length, bool allowRepeated = true)
		{
			int elementCount = elements.Count;
			var result = new T[length];
			if (allowRepeated)
			{
				lock (_randomEvent)
				{
					for (int i = 0; i < length; i++)
					{
						result[i] = elements[_randomEvent.Next(0, elementCount)];
					}
				}
			}
			else
			{
				if (length >= elementCount)
				{
					throw new ArgumentOutOfRangeException("The length must be less than the count of elements while allowing repeated.");
				}
				Dictionary<int, int> indexPatchDict = new Dictionary<int, int>(length / 2 + 1);//Give an estimate of capacity.
				lock (_randomEvent)
				{
					for (int i = 0; i < length; i++)
					{
						int index = _randomEvent.Next(0, elementCount - i);
						if (indexPatchDict.TryGetValue(index, out int realIndex))
						{
							result[i] = elements[realIndex];
						}
						else
						{
							result[i] = elements[index];
						}
						if (index != elementCount - i - 1)
						{
							if (indexPatchDict.TryGetValue(elementCount - i - 1, out int otherRealIndex))
							{
								indexPatchDict[index] = otherRealIndex;
							}
							else
							{
								indexPatchDict[index] = elementCount - i - 1;
							}
						}
					}
				}
			}
			return result;
		}
	}
}