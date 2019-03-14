using System;

namespace SpiderX.Extensions
{
	public static class DateTimeExtension
	{
		private const long TicksOfDbBirth = 621355968000000000;

		public static long ToTimestamp(this DateTime dt)
		{
			return (dt.Ticks - TicksOfDbBirth) / 10_000_000;
		}

		public static long ToTimestampMillisec(this DateTime dt)
		{
			return (dt.Ticks - TicksOfDbBirth) / 10_000;
		}

		public static int ToShortDateNumber(this DateTime dt)
		{
			return dt.Year * 100 + dt.Month;
		}

		public static int ToDateNumber(this DateTime dt)
		{
			return dt.Year * 10_000 + dt.Month * 100 + dt.Day;
		}

		public static long ToLongDateNumber(this DateTime dt)
		{
			return dt.Year * 10_000_000_000 + dt.Month * 100_000_000 + dt.Day * 1_000_000 + dt.Hour * 10_000 + dt.Minute * 100 + dt.Second;
		}

		public static long ToLongestDateNumber(this DateTime dt)
		{
			return ToLongDateNumber(dt) * 1000 + dt.Millisecond;
		}
	}
}