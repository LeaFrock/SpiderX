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

			public bool UseProxy { get; set; } = false;

			public override void Collect(string cityName, string keyword)
			{
				if (UseProxy)
				{
					throw new NotImplementedException();
				}
				LaGouResponseDataCollection dataCollection = new LaGouResponseDataCollection();
				using (var cookieClient = CreateCookiesWebClient())
				{
					Uri jobsListPageUri = PcWebApiProvider.GetJobListUri(cityName, keyword);
					//Init Cookies
					ResetHttpClientCookies(cookieClient, jobsListPageUri).ConfigureAwait(false).GetAwaiter().GetResult();
					using (var positionAjaxClient = CreatePositionAjaxWebClient())
					{
						//Preparing
						positionAjaxClient.DefaultRequestHeaders.Referrer = jobsListPageUri;
						Uri positionAjaxUri = PcWebApiProvider.GetPositionAjaxUri(cityName);
						HttpContent httpContent = PcWebApiProvider.GetPositionAjaxFormData(keyword, "1");
						//Start tasks
						var tasks = new Task[1];
						tasks[0] = GetResponseData(positionAjaxClient, positionAjaxUri, jobsListPageUri, httpContent, cookieClient.CookieContainer, dataCollection);
						Thread.Sleep(CommonTool.RandomEvent.Next(5000, 10000));
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
			}

			private SpiderWebClient CreatePositionAjaxWebClient()
			{
				SocketsHttpHandler httpHandler = new SocketsHttpHandler()
				{
					AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
					UseCookies = false,
					UseProxy = UseProxy
				};
				if (UseProxy)
				{
					//var uris = GetUrisFromDb();
					//var webProxy = CreateWebProxy(uris);
					//httpHandler.Proxy = webProxy;
					httpHandler.Proxy = HttpConsole.LocalWebProxy;
				}
				SpiderWebClient client = new SpiderWebClient(httpHandler);
				var headers = client.DefaultRequestHeaders;
				headers.Host = PcWebApiProvider.HomePageHost;
				headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
				headers.Add("Accept-Encoding", "br");
				headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
				headers.Add("X-Requested-With", "XMLHttpRequest");
				headers.Add("X-Anit-Forge-Code", "0");
				headers.Add("X-Anit-Forge-Token", "None");
				headers.Add("Origin", PcWebApiProvider.HomePageUrl);
				headers.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
				headers.Add("Cache-Control", "no-cache");
				headers.Add("Pragma", "no-cache");
				return client;
			}

			private SpiderWebClient CreateCookiesWebClient()
			{
				SocketsHttpHandler httpHandler = new SocketsHttpHandler()
				{
					AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
					UseCookies = true,
					UseProxy = UseProxy
				};
				if (UseProxy)
				{
					//var uris = GetUrisFromDb();
					//var webProxy = CreateWebProxy(uris);
					//httpHandler.Proxy = webProxy;
					httpHandler.Proxy = HttpConsole.LocalWebProxy;
				}
				SpiderWebClient client = new SpiderWebClient(httpHandler);
				var headers = client.DefaultRequestHeaders;
				headers.Host = PcWebApiProvider.HomePageHost;
				headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
				headers.Add("Accept-Encoding", "br");
				headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
				headers.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
				return client;
			}

			private static async Task GetResponseData(SpiderWebClient webClient, Uri targetUri, Uri cookieUri, HttpContent httpContent, CookieContainer cookieContainer, LaGouResponseDataCollection dataCollection)
			{
				var data = await GetResponseData(webClient, targetUri, cookieUri, httpContent, cookieContainer).ConfigureAwait(false);
				if (data != null)
				{
					dataCollection.AddResponseData(data);
				}
			}

			private static async Task<LaGouResponseData> GetResponseData(SpiderWebClient webClient, Uri targetUri, Uri cookieUri, HttpContent content, CookieContainer cookieContainer)
			{
				string cookies = cookieContainer.GetCookieHeader(targetUri);
				for (int i = 0; i < 3; i++)
				{
					var httpRequest = CreatePositionAjaxRequest(targetUri, content, cookies);
					var rspMsg = await webClient.SendAsync(httpRequest).ConfigureAwait(false);
					if (rspMsg == null || !rspMsg.IsSuccessStatusCode)
					{
						Thread.Sleep(5000);
						continue;
					}
					string text = await rspMsg.ToTextAsync().ConfigureAwait(false);
					//ShowConsoleMsg(text);
					if (string.IsNullOrEmpty(text))
					{
						Thread.Sleep(5000);
						continue;
					}
					if (text.Contains("频繁"))
					{
						await ResetHttpClientCookies(webClient, cookieUri);
						Thread.Sleep(5000);
						continue;
					}
					return PcWebApiProvider.CreateResponseData(text);
				}
				return null;
			}

			private static async Task ResetHttpClientCookies(SpiderWebClient webClient, Uri targetUri)
			{
				MakeCookiesExpired(webClient.CookieContainer, targetUri);
				try
				{
					var rspMsg = await webClient.GetAsync(targetUri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
					rspMsg.EnsureSuccessStatusCode();
				}
				catch
				{
					throw;
				}
			}

			private static HttpRequestMessage CreatePositionAjaxRequest(Uri targetUri, HttpContent content, string cookies)
			{
				HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, targetUri)
				{
					Content = content
				};
				httpRequest.Headers.Add("Cookie", cookies);
				return httpRequest;
			}

			private static void MakeCookiesExpired(CookieContainer container, Uri targetUri)
			{
				var cookies = container.GetCookies(targetUri).Cast<Cookie>();
				foreach (var cookie in cookies)
				{
					cookie.Expired = true;
				}
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