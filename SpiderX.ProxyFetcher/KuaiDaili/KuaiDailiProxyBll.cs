using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using SpiderX.Http;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	public sealed class KuaiDailiProxyBll : ProxyBll
	{
		private readonly static MediaTypeWithQualityHeaderValue _defaultAccept = new MediaTypeWithQualityHeaderValue("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
		private readonly static ProductInfoHeaderValue _defaultUserAgent = new ProductInfoHeaderValue(HttpConsole.DefaultPcUserAgent);

		internal override ProxyApiProvider ApiProvider { get; } = new KuaiDailiProxyApiProvider();

		public const string InhaUrlTemplate = "https://www.kuaidaili.com/free/inha/";//High Anonimity in China

		public override void Run(params string[] args)
		{
			Run();
		}

		public override async void Run()
		{
			base.Run();
			ProxyAgent pa = CreateProxyAgent();
			var entities = await GetProxyEntities(new SpiderWebClient(), InhaUrlTemplate, 10);
			int insertCount = pa.InsertProxyEntities(entities);
		}

		private async Task<List<SpiderProxyEntity>> GetProxyEntities(SpiderWebClient webClient, string urlTemplate, int maxPage)
		{
			List<SpiderProxyEntity> entities = new List<SpiderProxyEntity>(maxPage * 15);
			for (int p = 1; p <= maxPage; p++)
			{
				string url = GetRequestUrl(urlTemplate, p);
				string referer = GetRefererUrl(urlTemplate, p);
				HttpResponseMessage responseMsg = await GetResponseMessage(webClient, url, referer);
				string responseText = await responseMsg.Content.ReadAsStringAsync();
				var tempList = ApiProvider.GetProxyEntities(responseText);
				lock (entities)
				{
					entities.AddRange(tempList);
				}
				Thread.Sleep(RandomEvent.Next(4000, 6000));
			}
			return entities;
		}

		private async Task<HttpResponseMessage> GetResponseMessage(SpiderWebClient webClient, string requestUrl, string referer)
		{
			var request = CreateRequestMessage(requestUrl, referer);
			return await webClient.GetResponse(request);
		}

		private HttpRequestMessage CreateRequestMessage(string requestUrl, string referer)
		{
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
			request.Headers.Accept.Add(_defaultAccept);
			request.Headers.UserAgent.Add(_defaultUserAgent);
			request.Headers.Referrer = new Uri(referer);
			request.Headers.Host = ApiProvider.HomePageHost;
			return request;
		}

		private static string GetRequestUrl(string urlTemplate, int page) => urlTemplate + page.ToString();

		private static string GetRefererUrl(string urlTemplate, int page) => urlTemplate + Math.Max(1, page - RandomEvent.Next(1, 5)).ToString();
	}
}