using System;
using System.Collections.Concurrent;
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
				LaGouResponseDataCollection dataCollection = new LaGouResponseDataCollection();

				using (var client = CreateWebClient(false))
				{
					client.GetAsync(PcWebApiProvider.HomePageUrl, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false).GetAwaiter().GetResult();

					client.DefaultRequestHeaders.Referrer = referer;
					HttpContent urlFormData = PcWebApiProvider.GetRequestFormData(keyword, "1");
					var tempTask = client.PostAsync(uri, urlFormData).
						ContinueWith(async rspTask =>
						{
							var rspMsg = rspTask.Result;
							if (rspMsg == null)
							{
								return;
							}
							using (rspMsg)
							{
								if (!rspMsg.IsSuccessStatusCode)
								{
									return;
								}
								var rspData = await rspMsg.ToTextAsync().ContinueWith(txtTask =>
								{
									string text = txtTask.Result;
									if (string.IsNullOrEmpty(text))
									{
										ShowConsoleMsg("rspData =======null=============");
										return null;
									}
									ShowConsoleMsg(text);
									return PcWebApiProvider.CreateResponseData(text);
								}, TaskContinuationOptions.OnlyOnRanToCompletion);
								dataCollection.AddResponseData(rspData);
							}
						}, TaskContinuationOptions.OnlyOnRanToCompletion);
					try
					{
						ShowConsoleMsg("Task Wait Before");
						tempTask.Wait();
						ShowConsoleMsg("Task Wait Before 22222");
						tempTask.Result.Wait();
					}
					catch (Exception ex)
					{
						ShowConsoleMsg(ex.ToString());
						ShowConsoleMsg(ex.StackTrace);
					}
					ShowConsoleMsg("Task Wait After");
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