using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.Http;
using SpiderX.Http.Util;

namespace SpiderX.Business.Bilibili
{
	public partial class BilibiliLiveBll
	{
		private sealed class RoomCountPcWebCollector : CollectorBase
		{
			public override async Task<int> CollectRoomCountAsync(string areaIdStr)
			{
				Uri targetUri = RoomCountPcWebApiProvider.GetApiUri_GetLiveRoomCountByAreaID(areaIdStr);
				using var client = CreateWebClient();
				string rspText = null;
				try
				{
					rspText = await client.GetStringAsync(targetUri).ConfigureAwait(false);
				}
				catch
				{
					return 0;
				}
				if (!ValidateResponseTextOK(rspText))
				{
					return 0;
				}
				return RoomCountPcWebApiProvider.GetLiveRoomCount(rspText);
			}

			private static bool ValidateResponseTextOK(string rspText)
			{
				return rspText.EndsWith("}", StringComparison.Ordinal) && rspText.Contains("success", StringComparison.CurrentCultureIgnoreCase);
			}

			private static SpiderHttpClient CreateWebClient()
			{
				var client = new SpiderHttpClient();
				client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
				client.DefaultRequestHeaders.Add("Accept-Encoding", "br");
				client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
				client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
				client.DefaultRequestHeaders.Add("Origin", "https://live.bilibili.com");
				client.DefaultRequestHeaders.Referrer = RoomCountPcWebApiProvider.RefererUri;
				client.DefaultRequestHeaders.Host = "api.live.bilibili.com";
				client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
				client.DefaultRequestHeaders.Add("Pragma", "no-cache");
				return client;
			}
		}
	}
}