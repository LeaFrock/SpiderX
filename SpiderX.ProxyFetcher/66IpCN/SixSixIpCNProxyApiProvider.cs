using System.Collections.Generic;
using HtmlAgilityPack;
using SpiderX.Http;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	internal class SixSixIpCNProxyApiProvider : HtmlProxyApiProvider
	{
		public SixSixIpCNProxyApiProvider() : base()
		{
			HomePageHost = "www.66ip.cn";
			HomePageUrl = "http://www.66ip.cn/index.html";
		}

		public const string IpUrlTemplate = "http://www.66ip.cn/areaindex_{0}/{1}.html";

		public byte MaxPage { get; } = 2;

		public byte MaxAreaIndex { get; } = 34;

		public override IList<string> GetRequestUrls()
		{
			var urls = new List<string>(MaxAreaIndex * MaxPage);
			for (byte i = 1; i <= MaxAreaIndex; i++)
			{
				for (byte page = 1; page <= MaxPage; page++)
				{
					urls.Add(string.Format(IpUrlTemplate, i.ToString(), page.ToString()));
				}
			}
			return urls;
		}

		public override SpiderHttpClient CreateWebClient()
		{
			SpiderHttpClient client = new SpiderHttpClient();
			client.DefaultRequestHeaders.Host = HomePageHost;
			client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
			client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
			client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
			client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
			client.DefaultRequestHeaders.Add("Pragma", "no-cache");
			client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
			return client;
		}

		protected override List<SpiderProxyUriEntity> GetProxyEntities(HtmlDocument htmlDocument)
		{
			HtmlNodeCollection rows = htmlDocument.DocumentNode.SelectNodes("//div[@class='footer']//table[@bordercolor]//tr");
			if (rows == null || rows.Count < 2)
			{
				return null;
			}
			var entities = new List<SpiderProxyUriEntity>(rows.Count - 1);
			for (int i = 1; i < rows.Count; i++)
			{
				var entity = CreateProxyEntity(rows[i]);
				if (entity != null)
				{
					entities.Add(entity);
				}
			}
			return entities;
		}

		private static SpiderProxyUriEntity CreateProxyEntity(HtmlNode trNode)
		{
			var tdNodes = trNode.ChildNodes;
			HtmlNode portNode = tdNodes[1];
			string portText = portNode.InnerText.Trim();
			if (!int.TryParse(portText, out int port))
			{
				return null;
			}
			HtmlNode ipNode = tdNodes[0];
			string host = ipNode.InnerText.Trim();
			HtmlNode locationNode = tdNodes[2];
			string location = locationNode.InnerText?.Trim();
			HtmlNode anonymityNode = tdNodes[3];
			string anonymityText = anonymityNode.InnerText;
			byte anonymityDegree = (byte)(anonymityText.Contains("高匿") ? 3 : 0);
			return new SpiderProxyUriEntity()
			{
				Host = host,
				Port = port,
				AnonymityDegree = anonymityDegree,
				Category = 0,
				Location = location ?? string.Empty,
				ResponseMilliseconds = 10000
			};
		}
	}
}