using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using SpiderX.BusinessBase;
using SpiderX.Http;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
    public sealed class IpHaiProxyBll : BllBase
    {
        public const string HomePageUrl = "www.iphai.com";

        public const string NgUrl = "http://www.iphai.com/free/ng";//国内高匿
        public const string NpUrl = "http://www.iphai.com/free/np";//国内普通
        public const string WgUrl = "http://www.iphai.com/free/wg";//国外高匿
        public const string WpUrl = "http://www.iphai.com/free/wp";//国外普通

        public override string ClassName => GetType().Name;

        public override void Run(params string[] objs)
        {
            throw new NotImplementedException();
        }

        private readonly static Regex _doubleRegex = new Regex(@"\d+(\.\d+)?", RegexOptions.None, TimeSpan.FromMilliseconds(500));

        private readonly HtmlResponser _htmlResponser = new HtmlResponser();

        public override void Run()
        {
            base.Run();
            var ngEntities = GetProxyEntities(NgUrl);
            var npEntities = GetProxyEntities(NpUrl);
            var wgEntities = GetProxyEntities(WgUrl);
            var wpEntities = GetProxyEntities(WpUrl);
        }

        private List<SpiderProxyEntity> GetProxyEntities(string url)
        {
            var request = CreateRequest(url);
            var response = request.GetResponse();
            if (response == null)
            {
                return null;
            }
            var htmlDocument = _htmlResponser.LoadHtml(response);
            if (htmlDocument == null)
            {
                return null;
            }
            HtmlNodeCollection rows = htmlDocument.DocumentNode.SelectNodes("//table[contains(@class,'table')]/tr");
            if (rows == null || rows.Count <= 1)
            {
                return null;
            }
            var entities = new List<SpiderProxyEntity>(rows.Count - 1);
            for (int i = 1; i < rows.Count; i++)//第一行是列名，跳过
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
            string host = tdNodes[0].InnerText.Trim();
            string port = tdNodes[1].InnerText.Trim();
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

        private HttpWebRequest CreateRequest(string url)
        {
            var request = WebRequest.CreateHttp(url);
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Host = HomePageUrl;
            request.UserAgent = HttpConsole.DefaultPcUserAgent;
            request.Referer = NgUrl;
            request.Timeout = 5000;
            return request;
        }

        private static int ParseResponseMilliseconds(string text)
        {
            Match m = _doubleRegex.Match(text);
            if (!m.Success)
            {
                return 10000;
            }
            if (!double.TryParse(m.Value, out double result))
            {
                return 9999;
            }
            return (int)(result * 1000);
        }
    }
}