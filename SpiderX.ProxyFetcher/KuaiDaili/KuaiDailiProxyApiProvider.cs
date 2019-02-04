using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using SpiderX.Extensions;
using SpiderX.Http;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.ProxyFetcher
{
	internal sealed class KuaiDailiProxyApiProvider : HtmlProxyApiProvider
	{
		public KuaiDailiProxyApiProvider()
		{
			HomePageHost = "www.kuaidaili.com";
			HomePageUrl = "https://www.kuaidaili.com/";
		}

		public const string CnUrlTemplate = "https://www.kuaidaili.com/free/inha/";//High Anonimity in China

		public byte MaxPage { get; } = 10;

		public override SpiderWebClient CreateWebClient()
		{
			SpiderWebClient client = SpiderWebClient.CreateDefault();
			client.InnerClientHandler.UseProxy = false;
			client.DefaultRequestHeaders.Host = HomePageHost;
			client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			client.DefaultRequestHeaders.Add("Accept-Encoding", "br");
			client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
			client.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
			client.DefaultRequestHeaders.Add("DNT", "1");
			client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
			return client;
		}

		public override IList<string> GetRequestUrls()
		{
			string[] urls = new string[MaxPage];
			urls[0] = CnUrlTemplate;
			for (byte i = 1; i < MaxPage; i++)
			{
				urls[i] = CnUrlTemplate + (i + 1).ToString() + '/';
			}
			return urls;
		}

		protected override List<SpiderProxyEntity> GetProxyEntities(HtmlDocument htmlDocument)
		{
			HtmlNodeCollection rows = htmlDocument.DocumentNode.SelectNodes("//tbody/tr");
			if (rows.IsNullOrEmpty())
			{
				return null;
			}
			var entities = new List<SpiderProxyEntity>(rows.Count);
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

		private static SpiderProxyEntity CreateProxyEntity(HtmlNode node)
		{
			HtmlNode ipNode = node.SelectSingleNode("./td[contains(@data-title,'IP')]");
			if (ipNode == null)
			{
				return null;
			}
			HtmlNode portNode = node.SelectSingleNode("./td[contains(@data-title,'PORT')]");
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
			HtmlNode anonymityNode = node.SelectSingleNode("./td[contains(@data-title,'匿')]");
			string anonymityText = anonymityNode?.InnerText;
			byte anonymityDegree = (byte)((anonymityText != null && anonymityText.Contains("高匿")) ? 3 : 0);
			HtmlNode categoryNode = node.SelectSingleNode("./td[contains(@data-title,'类型')]");
			string categoryText = categoryNode?.InnerText;
			byte category = (byte)((categoryText != null && categoryText.Contains("Https", StringComparison.CurrentCultureIgnoreCase)) ? 1 : 0);
			HtmlNode locationNode = node.SelectSingleNode("./td[contains(@data-title,'位置')]");
			string location = locationNode?.InnerText?.Trim();
			HtmlNode responseIntervalNode = node.SelectSingleNode("./td[contains(@data-title,'响应')]");
			string responseIntervalText = responseIntervalNode?.InnerText;
			int responseMilliseconds = responseIntervalText == null ? 10000 : ParseResponseMilliseconds(responseIntervalText);
			return new SpiderProxyEntity()
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