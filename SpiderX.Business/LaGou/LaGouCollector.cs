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
			private const int MaxPage = 2;

			public bool UseProxy { get; set; } = false;

			public override LaGouResponseDataCollection Collect(string cityName, string keyword)
			{
				if (UseProxy)
				{
					throw new NotImplementedException();
				}
				LaGouResponseDataCollection dataCollection = new LaGouResponseDataCollection();
				using (var cookieClient = CreateCookiesWebClient())
				{
					Uri jobsListPageUri = PcWebApiProvider.GetJobListUri(cityName, keyword);
					cookieClient.DefaultRequestHeaders.Referrer = jobsListPageUri;
					//Init Cookies
					ResetHttpClientCookies(cookieClient, jobsListPageUri).ConfigureAwait(false).GetAwaiter().GetResult();
					Thread.Sleep(3333);
					using (var positionAjaxClient = CreatePositionAjaxWebClient())
					{
						//Preparing
						positionAjaxClient.DefaultRequestHeaders.Referrer = jobsListPageUri;
						Uri positionAjaxUri = PcWebApiProvider.GetPositionAjaxUri(cityName);
						var tasks = new Task[MaxPage];
						//Start tasks
						for (int i = 1; i <= MaxPage; i++)
						{
							HttpContent httpContent = PcWebApiProvider.GetPositionAjaxFormData(keyword, i.ToString());
							tasks[i - 1] = GetResponseData(positionAjaxClient, positionAjaxUri, jobsListPageUri, httpContent, cookieClient.CookieContainer, dataCollection);
							Thread.Sleep(CommonTool.RandomEvent.Next(5000, 10000));
						}
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
				foreach (var pos in dataCollection.Positions)
				{
					pos.Value.Keyword = keyword;
				}
				foreach (var com in dataCollection.Companies)
				{
					com.Value.CityName = cityName;
				}
				return dataCollection;
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
				var httpRequest = CreatePositionAjaxRequest(targetUri, content, cookies);
				var rspMsg = await webClient.SendAsync(httpRequest).ConfigureAwait(false);
				if (rspMsg == null || !rspMsg.IsSuccessStatusCode)
				{
					return null;
				}
				string text = await rspMsg.ToTextAsync().ConfigureAwait(false);
				ShowConsoleMsg(text);
				if (string.IsNullOrEmpty(text))
				{
					return null;
				}
				if (text.Contains("频繁"))
				{
					return null;
				}
				return PcWebApiProvider.CreateResponseData(text);
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
			public abstract LaGouResponseDataCollection Collect(string cityName, string keyword);

			protected static IEnumerable<Uri> GetUrisFromDb()
			{
				var pa = ProxyAgent<SqlServerProxyDbContext>.CreateInstance("SqlServerTest", true, c => new SqlServerProxyDbContext(c));
				var proxies = pa.SelectProxyEntities(e => e.Category == 1, 360).Select(e => e.Value);
				return proxies;
			}
		}
	}
}