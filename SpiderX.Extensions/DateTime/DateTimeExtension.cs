using System;

namespace SpiderX.Extensions
{
	public static class DateTimeExtension
	{
		public static long ToTimestamp(this DateTime dt)
		{
			return (dt.Ticks - 621355968000000000) / 10000000;
		}

		public static long ToTimestampMillisec(this DateTime dt)
		{
			return (dt.Ticks - 621355968000000000) / 10000;
		}
	}
}