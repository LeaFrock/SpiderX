using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SpiderX.Http;
using SpiderX.Http.Util;

namespace SpiderX.Business.Bilibili
{
	public partial class BilibiliLiveRoomCountBll
	{
		private sealed class PcWebCollector : CollectorBase
		{
			public override async Task<int> CollectAsync(string areaIdStr)
			{
				Uri apiUri = PcWebApiProvider.GetApiUri_GetLiveRoomCountByAreaID(areaIdStr);
				var proxyUriLoader = CreateProxyUriLoader();
				var proxySelector = new SimpleWebProxySelector(proxyUriLoader);
				string rspText = await HttpConsole.GetResponseTextByProxyAsync(apiUri, proxySelector, GetResponseTextByProxyAsync, ValidateResponseTextOK, 49);
				return PcWebApiProvider.GetLiveRoomCount(rspText);
			}

			private static async Task<string> GetResponseTextByProxyAsync(Uri targetUri, IWebProxy proxy)
			{
				using (var client = CreateWebClient(proxy))
				{
					try
					{
						return await client.GetStringAsync(targetUri);
					}
					catch
					{
						return null;
					}
				}
			}

			private static bool ValidateResponseTextOK(string rspText)
			{
				return rspText.EndsWith("}") && rspText.Contains("success", StringComparison.CurrentCultureIgnoreCase);
			}

			private static SpiderHttpClient CreateWebClient(IWebProxy proxy)
			{
				var client = new SpiderHttpClient(proxy);
				client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
				client.DefaultRequestHeaders.Add("Accept-Encoding", "br");
				client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
				client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
				client.DefaultRequestHeaders.Add("Origin", "https://live.bilibili.com");
				client.DefaultRequestHeaders.Referrer = PcWebApiProvider.RefererUri;
				client.DefaultRequestHeaders.Host = "api.live.bilibili.com";
				client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
				client.DefaultRequestHeaders.Add("Pragma", "no-cache");
				return client;
			}
		}
	}
}