using System;

namespace SpiderX.Tools
{
	public static class CommonTool
	{
		public readonly static Random RandomEvent = new Random();

		public readonly static DateTime DbMinTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

		public static DateTime FromTimestampMillisec(long millisec)
		{
			return DbMinTime.AddMilliseconds(millisec);
		}
	}
}