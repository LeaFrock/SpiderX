using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SpiderX.Extensions.Http;
using SpiderX.Http;
using SpiderX.Http.Util;
using SpiderX.Proxy;

namespace SpiderX.Business.LaGou
{
	public sealed partial class LaGouBll
	{
		private class PcWebCollector : CollectorBase
		{
			private const int MaxPage = 30;

			public override void Collect(string cityName, string keyword)
			{
				Uri uri = PcWebApiProvider.GetRequestUri(cityName);
				Uri referer = PcWebApiProvider.GetRefererUri(cityName, keyword);
				using (var client = CreateWebClient(false))
				{
					client.DefaultRequestHeaders.Referrer = referer;
					HttpContent urlFormData = PcWebApiProvider.GetRequestFormData(keyword, "1");
					var postTask = client.PostAsync(uri, urlFormData);
					var dealTask = postTask.ContinueWith(DoAfterPost, TaskContinuationOptions.OnlyOnRanToCompletion);
					try
					{
						var tempData = dealTask.ConfigureAwait(false).GetAwaiter().GetResult();
						if(tempData != null)
						{
							///ToDo
						}
					}
					catch
					{
					}
				}
			}

			private static LaGouResponseData DoAfterPost(Task<HttpResponseMessage> rspTask)
			{
				var rspMsg = rspTask.Result;
				if (rspMsg == null)
				{
					return null;
				}
				using (rspMsg)
				{
					if (!rspMsg.IsSuccessStatusCode)
					{
						return null;
					}
					var textTask = rspMsg.ToTextAsync().ContinueWith(t =>
					{
						string text = t.Result;
						if (string.IsNullOrEmpty(text))
						{
							return null;
						}
						return PcWebApiProvider.CreateResponseData(text);
					}, TaskContinuationOptions.OnlyOnRanToCompletion);
					try
					{
						return textTask.ConfigureAwait(false).GetAwaiter().GetResult();
					}
					catch
					{
						return null;
					}
				}
			}

			private SpiderWebClient CreateWebClient(bool useProxy = true)
			{
				SpiderWebClient client;
				if (useProxy)
				{
					var uris = GetUrisFromDb();
					var webProxy = CreateWebProxy(uris);
					client = new SpiderWebClient(webProxy);
				}
				else
				{
					client = new SpiderWebClient();
				}
				client.DefaultRequestHeaders.Host = PcWebApiProvider.HomePageHost;
				client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
				client.DefaultRequestHeaders.Add("Accept-Encoding", "br");
				client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
				client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
				client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
				client.DefaultRequestHeaders.Add("X-Anit-Forge-Code", "0");
				client.DefaultRequestHeaders.Add("X-Anit-Forge-Token", "None");
				client.DefaultRequestHeaders.Add("Origin", PcWebApiProvider.HomePageUrl);
				return client;
			}

			private static IWebProxy CreateWebProxy(IEnumerable<Uri> proxies)
			{
				DefaultSpiderProxyUriSelector uriSelector = new DefaultSpiderProxyUriSelector();
				uriSelector.Init(proxies);
				return new SpiderWebProxy(uriSelector);
			}
		}

		private abstract class CollectorBase
		{
			public abstract void Collect(string cityName, string keyword);

			protected static IEnumerable<Uri> GetUrisFromDb()
			{
				var pa = ProxyAgent<SqlServerProxyDbContext>.CreateInstance("SqlServerTest", true, c => new SqlServerProxyDbContext(c));
				var proxies = pa.SelectProxyEntities(e => e.Category == 1, 360).Select(e => e.Value);
				return proxies;
			}
		}
	}
}