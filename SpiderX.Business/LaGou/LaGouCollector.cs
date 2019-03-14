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
				Uri jobsListGetUri = PcWebApiProvider.GetJobsListUri(cityName, keyword);
				using (var jobsListClient = CreateJobsListWebClient(false))
				{
					var jobRsp = jobsListClient.GetAsync(jobsListGetUri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false).GetAwaiter().GetResult();
					if (!jobRsp.IsSuccessStatusCode || !jobRsp.Headers.TryGetValues("Set-Cookie", out var setCookies))
					{
						return;
					}

					Uri positionAjaxUri = PcWebApiProvider.GetPositionAjaxUri(cityName);
					LaGouResponseDataCollection dataCollection = new LaGouResponseDataCollection();
					using (var positionAjaxClient = CreatePositionAjaxWebClient(false))
					{
						HttpContent urlFormData = PcWebApiProvider.GetPositionAjaxFormData(keyword, "1");

						HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, positionAjaxUri)
						{
							Content = urlFormData
						};
						requestMessage.Headers.Referrer = jobsListGetUri;
						requestMessage.Headers.Add("Cookie", string.Concat(setCookies));

						var tempTask = positionAjaxClient.SendAsync(requestMessage).
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
											return null;
										}
										//ShowConsoleMsg(text);
										return PcWebApiProvider.CreateResponseData(text);
									}, TaskContinuationOptions.OnlyOnRanToCompletion);
									dataCollection.AddResponseData(rspData);
								}
							}, TaskContinuationOptions.OnlyOnRanToCompletion);
					}
				}
			}

			private SpiderWebClient CreatePositionAjaxWebClient(bool useProxy = true)
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
				client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
				return client;
			}

			private SpiderWebClient CreateJobsListWebClient(bool useProxy = true)
			{
				SocketsHttpHandler httpHandler = new SocketsHttpHandler()
				{
					AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
					UseCookies = true,
					UseProxy = useProxy
				};
				if (useProxy)
				{
					var uris = GetUrisFromDb();
					httpHandler.Proxy = CreateWebProxy(uris);
				}

				SpiderWebClient client = new SpiderWebClient(httpHandler);
				client.DefaultRequestHeaders.Host = PcWebApiProvider.HomePageHost;
				client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
				client.DefaultRequestHeaders.Add("Accept-Encoding", "br");
				client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
				client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
				client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
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