using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using SpiderX.Extensions;
using SpiderX.Http;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.ProxyFetcher
{
	internal sealed class KxDailiProxyApiProvider : HtmlProxyApiProvider
	{
		public KxDailiProxyApiProvider() : base()
		{
			HomePageHost = "ip.kxdaili.com";
			HomePageUrl = "http://ip.kxdaili.com/";
		}

		public const string CnUrlTemplate = "http://ip.kxdaili.com/dailiip/1/{0}.html";

		public const string DefaultRefererUrl = "http://ip.kxdaili.com/dailiip.html";

		public byte MaxPage { get; } = 10;

		public override IList<string> GetRequestUrls()
		{
			string[] urls = new string[MaxPage];
			for (byte i = 0; i < MaxPage; i++)
			{
				urls[i] = string.Format(CnUrlTemplate, (i + 1).ToString());
			}
			return urls;
		}

		public override SpiderWebClient CreateWebClient()
		{
			SpiderWebClient client = new SpiderWebClient();
			client.DefaultRequestHeaders.Host = HomePageHost;
			client.DefaultRequestHeaders.Referrer = new Uri(DefaultRefererUrl);
			client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
			client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
			client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
			return client;
		}

		protected override List<SpiderProxyUriEntity> GetProxyEntities(HtmlDocument htmlDocument)
		{
			HtmlNodeCollection rows = htmlDocument.DocumentNode.SelectNodes("//tbody/tr");
			if (rows.IsNullOrEmpty())
			{
				return null;
			}
			var entities = new List<SpiderProxyUriEntity>(rows.Count);
			foreach (var row in rows)
			{
				var entity = CreateProxyEntity(row);
				if (entity != null)
				{
					entities.Add(entity);
				}
			}
			return entities;
		}

		private static SpiderProxyUriEntity CreateProxyEntity(HtmlNode node)
		{
			var tdNodes = node.SelectNodes("./td");
			if (tdNodes == null || tdNodes.Count != 7)
			{
				return null;
			}
			HtmlNode portNode = tdNodes[1];
			string portText = portNode.InnerText?.Trim();
			if (string.IsNullOrEmpty(portText) || !int.TryParse(portText, out int port))
			{
				return null;
			}
			HtmlNode ipNode = tdNodes[0];
			string host = ipNode.InnerText?.Trim();
			if (string.IsNullOrEmpty(host))
			{
				return null;
			}
			HtmlNode anonymityNode = tdNodes[2];
			string anonymityText = anonymityNode?.InnerText;
			byte anonymityDegree = (byte)((anonymityText != null && anonymityText.Contains("高匿")) ? 3 : 0);
			HtmlNode categoryNode = tdNodes[3];
			string categoryText = categoryNode?.InnerText;
			byte category = (byte)((categoryText != null && categoryText.Contains("Https", StringComparison.CurrentCultureIgnoreCase)) ? 1 : 0);
			HtmlNode locationNode = tdNodes[5];
			string location = locationNode?.InnerText?.Trim();
			HtmlNode responseIntervalNode = tdNodes[4];
			string responseIntervalText = responseIntervalNode?.InnerText;
			int responseMilliseconds = responseIntervalText == null ? 10000 : ParseResponseMilliseconds(responseIntervalText);
			return new SpiderProxyUriEntity()
			{
				Host = host,
				Port = port,
				AnonymityDegree = anonymityDegree,
				Category = category,
				Location = location,
				ResponseMilliseconds = responseMilliseconds
			};
		}

		private static int ParseResponseMilliseconds(string text)
		{
			double seconds = StringTool.MatchDouble(text, 10d);
			return (int)(seconds * 1000);
		}
	}
}