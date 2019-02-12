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

		public const string IpUrlTemplate = "http://www.ip3366.net/free/?stype={0}&page={1}";

		public const string DefaultRefererUrl = "http://www.ip3366.net/free/";

		public byte MaxPage { get; } = 7;

		public override SpiderWebClient CreateWebClient()
		{
			SpiderWebClient client = SpiderWebClient.CreateDefault();
			client.InnerClientHandler.UseProxy = false;
			client.DefaultRequestHeaders.Host = HomePageHost;
			client.DefaultRequestHeaders.Referrer = new Uri(DefaultRefererUrl);
			client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
			client.DefaultRequestHeaders.Add("Pragma", "no-cache");
			client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
			client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
			client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
			return client;
		}

		public override IList<string> GetRequestUrls()
		{
			string[] stypes = new string[] { "1", "3" };
			List<string> urls = new List<string>(stypes.Length * MaxPage);
			foreach (string stype in stypes)
			{
				for (byte i = 1; i <= MaxPage; i++)
				{
					urls.Add(string.Format(IpUrlTemplate, stype, i.ToString()));
				}
			}
			return urls;
		}

		protected override List<SpiderProxyUriEntity> GetProxyEntities(HtmlDocument htmlDocument)
		{
			HtmlNodeCollection rows = htmlDocument.DocumentNode.SelectNodes("//tbody/tr");
			if (rows.IsNullOrEmpty())
			{
				return null;
			}
			var entities = new List<SpiderProxyUriEntity>(rows.Count);
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

		private static string FixLocation(string loc)
		{
			if (string.IsNullOrEmpty(loc))
			{
				return string.Empty;
			}
			const string keyword = "高匿_";
			int index = loc.IndexOf(keyword);
			if (index >= 0)
			{
				return loc.Substring(index + keyword.Length);
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