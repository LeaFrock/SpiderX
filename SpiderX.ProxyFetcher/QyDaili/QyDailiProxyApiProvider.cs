using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using SpiderX.Extensions;
using SpiderX.Http;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.ProxyFetcher
{
	internal class QyDailiProxyApiProvider : HtmlProxyApiProvider
	{
		public QyDailiProxyApiProvider() : base()
		{
			HomePageHost = "www.qydaili.com";
			HomePageUrl = "http://www.qydaili.com/";
		}

		public const string IpUrlTemplate = "http://www.qydaili.com/free/?action={0}&page={1}";

		public const string DefaultRefererUrl = "http://www.qydaili.com/free/";

		public byte MaxPage { get; } = 10;

		public override IList<string> GetRequestUrls()
		{
			string[] actions = new string[] { "china", "unchina" };
			var urls = new List<string>(actions.Length * MaxPage);
			foreach (var action in actions)
			{
				for (byte i = 0; i < MaxPage; i++)
				{
					urls.Add(string.Format(IpUrlTemplate, action, (i + 1).ToString()));
				}
			}
			return urls;
		}

		public override SpiderWebClient CreateWebClient()
		{
			SpiderWebClient client = SpiderWebClient.CreateDefault();
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
			HtmlNode ipNode = node.SelectSingleNode("./td[contains(@data-title,'IP')]/text()");
			if (ipNode == null)
			{
				return null;
			}
			HtmlNode portNode = node.SelectSingleNode("./td[contains(@data-title,'PORT')]/text()");
			if (portNode == null)
			{
				return null;
			}
			string portText = portNode.InnerText.Trim();
			if (!int.TryParse(portText, out int port))
			{
				return null;
			}
			string host = ipNode.InnerText.Trim();
			HtmlNode anonymityNode = node.SelectSingleNode("./td[contains(@data-title,'匿')]/text()");
			string anonymityText = anonymityNode?.InnerText;
			byte anonymityDegree = (byte)((anonymityText != null && anonymityText.Contains("高匿")) ? 3 : 0);
			HtmlNode categoryNode = node.SelectSingleNode("./td[contains(@data-title,'类型')]/text()");
			string categoryText = categoryNode?.InnerText;
			byte category = (byte)((categoryText != null && categoryText.Contains("Https", StringComparison.CurrentCultureIgnoreCase)) ? 1 : 0);
			HtmlNode locationNode = node.SelectSingleNode("./td[contains(@data-title,'位置')]/text()");
			string location = locationNode?.InnerText?.Trim();
			HtmlNode responseIntervalNode = node.SelectSingleNode("./td[contains(@data-title,'响应')]/text()");
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