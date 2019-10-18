using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderX.Business.Bilibili
{
	internal sealed class BilibiliLiveRoomCount
	{
		public int Id { get; set; }

		public int RoomCount { get; set; }

		public int Month { get; set; }

		public int Day { get; set; }

		public int Hour { get; set; }

		public static BilibiliLiveRoomCount Create(int count)
		{
			DateTime now = DateTime.Now;
			var b = new BilibiliLiveRoomCount()
			{
				RoomCount = count,
				Month = now.Month,
				Day = now.Day,
				Hour = now.Hour
			};
			return b;
		}
	}
}