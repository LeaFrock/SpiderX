using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using SpiderX.Extensions;
using SpiderX.Http;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	public sealed class KuaiDailiProxyBll : ProxyBll
	{
		internal override ProxyApiProvider ApiProvider { get; } = new KuaiDailiProxyApiProvider();

		public const string InhaUrlTemplate = "https://www.kuaidaili.com/free/inha/";//High Anonimity in China

		public override void Run(params string[] args)
		{
			Run();
		}

		public override void Run()
		{
			base.Run();
			ProxyAgent pa = CreateProxyAgent();
			using (SpiderWebClient webClient = CreateWebClient())
			{
				var entities = GetProxyEntities(webClient, InhaUrlTemplate, 10);
				int insertCount = pa.InsertProxyEntities(entities);
			}
		}

		private List<SpiderProxyEntity> GetProxyEntities(SpiderWebClient webClient, string urlTemplate, int maxPage)
		{
			List<SpiderProxyEntity> entities = new List<SpiderProxyEntity>(maxPage * 15);
			for (int p = 1; p <= maxPage; p++)
			{
				string url = GetRequestUrl(urlTemplate, p);
				string referer = GetRefererUrl(urlTemplate, p);
				var responseTask = GetResponseMessageAsync(webClient, url, referer);
				responseTask?.ContinueWith(responseMsg =>
					{
						using (HttpResponseMessage responseMessage = responseMsg.Result)
						{
							if (!responseMessage.IsSuccessStatusCode)
							{
								return;
							}
							Task<string> readTask = responseMessage.Content.ReadAsStringAsync();
							readTask.ContinueWith(text =>
							{
								var tempList = ApiProvider.GetProxyEntities(text.Result);
								if (!tempList.IsNullOrEmpty())
								{
									lock (entities)
									{
										entities.AddRange(tempList);
									}
								}
							});
						}
					});
				Thread.Sleep(RandomEvent.Next(4000, 6000));
			}
			return entities;
		}

		private async Task<HttpResponseMessage> GetResponseMessageAsync(SpiderWebClient webClient, string requestUrl, string referer)
		{
			var request = CreateRequestMessage(requestUrl, referer);
			try
			{
				return await webClient.SendAsync(request);
			}
			finally
			{
				request.Dispose();
			}
		}

		private HttpRequestMessage CreateRequestMessage(string requestUrl, string referer)
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
			request.Headers.Referrer = new Uri(referer);
			request.Headers.Host = ApiProvider.HomePageHost;
			return request;
		}

		private SpiderWebClient CreateWebClient()
		{
			SpiderWebClient client = new SpiderWebClient()
			{
				Timeout = TimeSpan.FromMilliseconds(5000)
			};
			client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
			client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
			client.DefaultRequestHeaders.Connection.Add("keep-alive");
			client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
			client.DefaultRequestHeaders.Pragma.Add(new NameValueHeaderValue("no-cache"));
			client.DefaultRequestHeaders.Host = ApiProvider.HomePageHost;
			client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36");
			return client;
		}

		private static string GetRequestUrl(string urlTemplate, int page) => urlTemplate + page.ToString();

		private static string GetRefererUrl(string urlTemplate, int page) => urlTemplate + Math.Max(1, page - RandomEvent.Next(1, 5)).ToString();
	}
}