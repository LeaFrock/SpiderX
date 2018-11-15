using System;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;
using SpiderX.Http;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.ProxyFetcher
{
    public sealed class IpHaiProxyBll : ProxyBll
    {
        public const string HomePageHost = "www.iphai.com";

        public const string NgUrl = "http://www.iphai.com/free/ng";//国内高匿
        public const string NpUrl = "http://www.iphai.com/free/np";//国内普通
        public const string WgUrl = "http://www.iphai.com/free/wg";//国外高匿
        public const string WpUrl = "http://www.iphai.com/free/wp";//国外普通

        public override void Run(params string[] args)
        {
            Run();
        }

        public override void Run()
        {
            base.Run();
            ProxyAgent pa = CreateProxyAgent();
            var ngEntities = GetProxyEntities(NgUrl);
            var npEntities = GetProxyEntities(NpUrl);
            var wgEntities = GetProxyEntities(WgUrl);
            var wpEntities = GetProxyEntities(WpUrl);
            List<SpiderProxyEntity> totalEntities = new List<SpiderProxyEntity>(ngEntities.Count + npEntities.Count + wgEntities.Count + wpEntities.Count);
            totalEntities.AddRange(ngEntities);
            totalEntities.AddRange(npEntities);
            totalEntities.AddRange(wgEntities);
            totalEntities.AddRange(wpEntities);
            int insertCount = pa.InsertProxyEntities(totalEntities);
        }

        private List<SpiderProxyEntity> GetProxyEntities(string url)
        {
            var request = CreateRequest(url);
            var response = request.GetResponse();
            if (response == null)
            {
                return new List<SpiderProxyEntity>(0);
            }
            var htmlDocument = _defaultHtmlResponser.LoadHtml(response);
            if (htmlDocument == null)
            {
                return new List<SpiderProxyEntity>(0);
            }
            HtmlNodeCollection rows = htmlDocument.DocumentNode.SelectNodes("//table[contains(@class,'table')]/tr");
            if (rows == null || rows.Count <= 1)
            {
                return new List<SpiderProxyEntity>(0);
            }
            var entities = new List<SpiderProxyEntity>(rows.Count - 1);
            for (int i = 1; i < rows.Count; i++)//Skip the ColumnName row.
            {
                var entity = CreateProxyEntity(rows[i]);
                if (entity != null)
                {
                    entities.Add(entity);
                }
            }
            return entities;
        }

        private SpiderProxyEntity CreateProxyEntity(HtmlNode trNode)
        {
            var tdNodes = trNode.SelectNodes("./td");
            if (tdNodes == null || tdNodes.Count < 6)
            {
                return null;
            }
            string portText = tdNodes[1].InnerText.Trim();
            if (!int.TryParse(portText, out int port))
            {
                return null;
            }
            string host = tdNodes[0].InnerText.Trim();
            string anonymityDegreeText = tdNodes[2].InnerText;
            int anonymityDegree = anonymityDegreeText.Contains("高") ? 3 : 1;
            string categoryText = tdNodes[3].InnerText;
            int category = categoryText.Contains("HTTPS", StringComparison.CurrentCultureIgnoreCase) ? 1 : 0;
            string location = tdNodes[4].InnerText.Trim();
            string responseTimespanText = tdNodes[5].InnerText;
            int responseMilliseconds = ParseResponseMilliseconds(responseTimespanText);
            //if(...)return null;
            SpiderProxyEntity entity = new SpiderProxyEntity()
            {
                Host = host,
                Port = port,
                AnonymityDegree = anonymityDegree,
                Category = category,
                Location = location,
                ResponseMilliseconds = responseMilliseconds
            };
            return entity;
        }

        private HttpWebRequest CreateRequest(string url, IWebProxy proxy = null)
        {
            var request = WebRequest.CreateHttp(url);
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Host = HomePageHost;
            request.UserAgent = HttpConsole.DefaultPcUserAgent;
            request.Referer = NgUrl;
            request.Timeout = 5000;
            request.Proxy = proxy;
            return request;
        }

        private static int ParseResponseMilliseconds(string text)
        {
            if (!StringTool.TryMatchDouble(text, out double result))
            {
                return 10000;
            }
            return (int)(result * 1000);
        }
    }
}