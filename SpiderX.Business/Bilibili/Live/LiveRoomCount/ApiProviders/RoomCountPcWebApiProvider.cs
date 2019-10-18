using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using SpiderX.Tools;

namespace SpiderX.Business.Bilibili
{
	public partial class BilibiliLiveBll
	{
		private static class RoomCountPcWebApiProvider
		{
			public readonly static Uri HomePageUri = new Uri("https://live.bilibili.com");

			public readonly static Uri RefererUri = new Uri("https://live.bilibili.com/all");

			public static Uri GetApiUri_GetLiveRoomCountByAreaID(string areaIdStr)
			{
				string url = "https://api.live.bilibili.com/room/v1/Area/getLiveRoomCountByAreaID?areaId=" + areaIdStr;
				return new Uri(url);
			}

			public static int GetLiveRoomCount(string response)
			{
				var source = JsonTool.DeserializeObject<JToken>(response);
				var data = source?.Value<JToken>("data");
				if (data == null)
				{
					return 0;
				}
				int num = data.Value<int>("num");
				return num;
			}
		}
	}
}