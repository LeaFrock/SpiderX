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
            SpiderWebClient webClient = CreateWebClient();
            var entities = await GetProxyEntities(webClient, InhaUrlTemplate, 10);
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
            request.Headers.Referrer = new Uri(referer);
            request.Headers.Host = ApiProvider.HomePageHost;
            return request;
        }

        private static string GetRequestUrl(string urlTemplate, int page) => urlTemplate + page.ToString();

        private static string GetRefererUrl(string urlTemplate, int page) => urlTemplate + Math.Max(1, page - RandomEvent.Next(1, 5)).ToString();

        private static SpiderWebClient CreateWebClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8d));
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36");
            return new SpiderWebClient(client);
        }
    }
}