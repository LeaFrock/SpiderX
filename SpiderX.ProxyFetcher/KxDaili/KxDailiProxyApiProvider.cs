using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using SpiderX.Extensions;
using SpiderX.Http;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.ProxyFetcher
{
	internal sealed class KxDailiProxyApiProvider : ProxyApiProvider
	{
		public KxDailiProxyApiProvider() : base()
		{
			HomePageHost = "ip.kxdaili.com";
			HomePageUrl = "http://ip.kxdaili.com/";
		}

		public const string IpUrlTemplate = "http://ip.kxdaili.com/dailiip/1/{0}.html";
		public const string DefaultRefererUrl = "http://ip.kxdaili.com/dailiip.html";

		public override SpiderWebClient CreateWebClient()
		{
			SpiderWebClient client = SpiderWebClient.CreateDefault();
			client.InnerClientHandler.UseProxy = false;
			client.DefaultRequestHeaders.Host = HomePageHost;
			client.DefaultRequestHeaders.Referrer = new Uri(DefaultRefererUrl);
			client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
			client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
			//client.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
			client.DefaultRequestHeaders.Add("Pragma", "no-cache");
			client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
			//client.DefaultRequestHeaders.Add("DNT", "1");
			client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
			client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
			return client;
		}

		public override List<SpiderProxyEntity> GetProxyEntities(string response)
		{
			var htmlDocument = HtmlTool.LoadFromText(response);
			if (htmlDocument == null)
			{
				return null;
			}
			return GetProxyEntities(htmlDocument);
		}

		public override List<SpiderProxyEntity> GetProxyEntities<T>(T reader)
		{
			var htmlDocument = HtmlTool.LoadFromTextReader(reader);
			if (htmlDocument == null)
			{
				return null;
			}
			return GetProxyEntities(htmlDocument);
		}

		private static List<SpiderProxyEntity> GetProxyEntities(HtmlDocument htmlDocument)
		{
			HtmlNodeCollection rows = htmlDocument.DocumentNode.SelectNodes(".//tbody//tr");
			if (rows.IsNullOrEmpty())
			{
				return null;
			}
			var entities = new List<SpiderProxyEntity>(rows.Count);
			for (int i = 0; i < rows.Count; i++)
			{
				var entity = CreateProxyEntity(rows[i]);
				if (entity != null)
				{
					entities.Add(entity);
				}
			}
			return entities;
		}

		private static SpiderProxyEntity CreateProxyEntity(HtmlNode node)
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