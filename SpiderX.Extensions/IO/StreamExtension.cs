using System.IO;

namespace SpiderX.Extensions.IO
{
	public static class StreamExtension
	{
		public static T CloneNew<T>(this Stream source, bool resetOffset = true) where T : Stream, new()
		{
			T target = new T();
			source.CopyTo(target);
			//After copying, put the pointer back to the beginning, otherwise the StreamReader will always read string.Empty
			target.Seek(0, SeekOrigin.Begin);
			if (resetOffset)
			{
				source.Seek(0, SeekOrigin.Begin);
			}
			return target;
		}
	}
}