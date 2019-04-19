using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using SpiderX.Http;
using SpiderX.Http.Util;

namespace SpiderX.Business.Bilibili
{
    public partial class BilibiliLiveCountBll
    {
        private sealed class PcWebCollector : CollectorBase
        {
            public override int Collect(string areaIdStr)
            {
                Uri apiUri = PcWebApiProvider.GetApiUri_GetLiveRoomCountByAreaID(areaIdStr);
                HttpRequestMessage createRequestMessage() => new HttpRequestMessage(HttpMethod.Get, apiUri) { Version = HttpVersion.Version11 };
                var requestFactory = new HttpRequestFactory(CreateWebClient, createRequestMessage);
                var proxyUriLoader = CreateProxyUriLoader();
                var proxySelector = new SimpleWebProxySelector(proxyUriLoader);
                string rspText = HttpConsole.GetResponseTextByProxy(requestFactory, proxySelector, ValidateResponseTextOK, 49);
                return PcWebApiProvider.GetLiveRoomCount(rspText);
            }

            private static bool ValidateResponseTextOK(string rspText)
            {
                return rspText.EndsWith("}") && rspText.Contains("success", StringComparison.CurrentCultureIgnoreCase);
            }

            private static SpiderWebClient CreateWebClient(IWebProxy proxy)
            {
                var client = new SpiderWebClient(proxy);
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