using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SpiderX.Extensions.Http;
using SpiderX.Http;
using SpiderX.Http.Util;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.Business.LaGou
{
	public sealed partial class LaGouBll
	{
		private class PcWebCollector : CollectorBase
		{
			private const int MaxPage = 30;

			public override void Collect(string cityName, string keyword)
			{
				CookieContainer container = new CookieContainer();

				Uri jobsListPageUri = PcWebApiProvider.GetJobListUri(cityName, keyword);
				using (var positionAjaxClient = CreateWebClient(false, container))
				{
					//Init Cookies
					try
					{
						ResetHttpClientCookies(positionAjaxClient, jobsListPageUri).ConfigureAwait(false).GetAwaiter().GetResult();
					}
					catch
					{
						throw;
					}
					Thread.Sleep(2000);
					var aaa = container.GetCookies(jobsListPageUri).Cast<Cookie>().ToList();
					var bbb = container.GetCookieHeader(jobsListPageUri);
					//Preparing
					Uri positionAjaxUri = PcWebApiProvider.GetPositionAjaxUri(cityName);
					var ccc = container.GetCookieHeader(positionAjaxUri);
					HttpContent httpContent = PcWebApiProvider.GetPositionAjaxFormData(keyword, "1");
					LaGouResponseDataCollection dataCollection = new LaGouResponseDataCollection();
					//Start tasks
					var tasks = new Task[1];
					tasks[0] = GetResponseData(positionAjaxClient, positionAjaxUri, jobsListPageUri, httpContent, dataCollection);
					//Wait all tasks
					try
					{
						Task.WaitAll(tasks);
					}
					catch
					{
						throw;
					}
				}
			}

			private static async Task GetResponseData(SpiderWebClient webClient, Uri target, Uri homePageUri, HttpContent httpContent, LaGouResponseDataCollection dataCollection)
			{
				var data = await GetResponseData(webClient, target, homePageUri, httpContent).ConfigureAwait(false);
				if (data != null)
				{
					dataCollection.AddResponseData(data);
				}
			}

			private static async Task<LaGouResponseData> GetResponseData(SpiderWebClient webClient, Uri target, Uri homePageUri, HttpContent httpContent)
			{
				for (int i = 0; i < 3; i++)
				{
					var request = CreatePositionAjaxRequest(target, httpContent);
					var rspMsg = await webClient.SendAsync(request).ConfigureAwait(false);
					if (rspMsg == null || !rspMsg.IsSuccessStatusCode)
					{
						Thread.Sleep(5000);
						continue;
					}
					string text = await rspMsg.ToTextAsync().ConfigureAwait(false);
					if (string.IsNullOrEmpty(text))
					{
						Thread.Sleep(5000);
						continue;
					}
					if (text.Contains("频繁"))
					{
						await ResetHttpClientCookies(webClient, homePageUri);
						Thread.Sleep(5000);
						continue;
					}
					//ShowConsoleMsg(text);
					return PcWebApiProvider.CreateResponseData(text);
				}
				return null;
			}

			private static async Task ResetHttpClientCookies(SpiderWebClient webClient, Uri target)
			{
				HttpRequestMessage requestMessage = CreateJobListPageRequest(target);
				try
				{
					var rspMsg = await webClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
					rspMsg.EnsureSuccessStatusCode();
				}
				catch
				{
					throw;
				}
			}

			private static HttpRequestMessage CreateJobListPageRequest(Uri target)
			{
				HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, target);
				requestMessage.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
				return requestMessage;
			}

			private static HttpRequestMessage CreatePositionAjaxRequest(Uri target, HttpContent httpContent)
			{
				HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, target)
				{
					Content = httpContent
				};
				requestMessage.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
				requestMessage.Headers.Add("X-Requested-With", "XMLHttpRequest");
				requestMessage.Headers.Add("X-Anit-Forge-Code", "0");
				requestMessage.Headers.Add("X-Anit-Forge-Token", "None");
				requestMessage.Headers.Add("Origin", PcWebApiProvider.HomePageUrl);
				return requestMessage;
			}

			private SpiderWebClient CreateWebClient(bool useProxy = true, CookieContainer cc = null)
			{
				SocketsHttpHandler httpHandler = new SocketsHttpHandler()
				{
					AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
					UseCookies = true,
					CookieContainer = cc,
					UseProxy = useProxy
				};
				if (useProxy)
				{
					//var uris = GetUrisFromDb();
					//var webProxy = CreateWebProxy(uris);
					//httpHandler.Proxy = webProxy;
					httpHandler.Proxy = HttpConsole.LocalWebProxy;
				}
				SpiderWebClient client = new SpiderWebClient(httpHandler);
				client.DefaultRequestHeaders.Host = PcWebApiProvider.HomePageHost;
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