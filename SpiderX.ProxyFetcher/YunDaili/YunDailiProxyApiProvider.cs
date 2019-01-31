using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using SpiderX.Extensions;
using SpiderX.Http;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.ProxyFetcher
{
	internal sealed class YunDailiProxyApiProvider : HtmlProxyApiProvider
	{
		public YunDailiProxyApiProvider() : base()
		{
			HomePageHost = "www.ip3366.net";
			HomePageUrl = "http://www.ip3366.net/";
		}

		public const string CnIpUrlTemplate = "http://www.ip3366.net/free/?stype=1&page=";
		public const string OtherIpUrlTemplate = "http://www.ip3366.net/free/?stype=3&page=";

		public const string DefaultReferer = "http://www.ip3366.net/free/";

		public override SpiderWebClient CreateWebClient()
		{
			SpiderWebClient client = SpiderWebClient.CreateDefault();
			client.InnerClientHandler.UseProxy = false;
			client.DefaultRequestHeaders.Host = HomePageHost;
			client.DefaultRequestHeaders.Referrer = new Uri(DefaultReferer);
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

		protected override List<SpiderProxyEntity> GetProxyEntities(HtmlDocument htmlDocument)
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
			string anonymityText = anonymityNode.InnerText;
			byte anonymityDegree = (byte)((anonymityText != null && anonymityText.Contains("高匿")) ? 3 : 0);
			HtmlNode categoryNode = tdNodes[3];
			string categoryText = categoryNode.InnerText;
			byte category = (byte)((categoryText != null && categoryText.Contains("Https", StringComparison.CurrentCultureIgnoreCase)) ? 1 : 0);
			HtmlNode locationNode = tdNodes[4];
			string location = FixLocation(locationNode.InnerText?.Trim());
			HtmlNode responseIntervalNode = tdNodes[5];
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

		private static string FixLocation(string loc)
		{
			if (string.IsNullOrEmpty(loc))
			{
				return string.Empty;
			}
			const string word = "高匿_";
			int index = loc.IndexOf(word);
			if (index >= 0)
			{
				return loc.Substring(index + word.Length);
			}
			return loc;
		}

		private static int ParseResponseMilliseconds(string text)
		{
			double seconds = StringTool.MatchDouble(text, 10d);//The result may be Zero.
			return Math.Max(500, (int)(seconds * 1000));
		}
	}
}