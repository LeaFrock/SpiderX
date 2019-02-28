using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using SpiderX.Extensions;
using SpiderX.Http;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.ProxyFetcher
{
	internal sealed class FeiyiProxyApiProvider : HtmlProxyApiProvider
	{
		public FeiyiProxyApiProvider() : base()
		{
			HomePageHost = "www.feiyiproxy.com";
			HomePageUrl = "http://www.feiyiproxy.com/";
		}

		public const string IpUrl = "http://www.feiyiproxy.com/?page_id=1457";

		public override SpiderWebClient CreateWebClient()
		{
			SpiderWebClient client = new SpiderWebClient();
			client.DefaultRequestHeaders.Host = HomePageHost;
			client.DefaultRequestHeaders.Referrer = new Uri(HomePageUrl);
			client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
			client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
			client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
			client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
			return client;
		}

		public override IList<string> GetRequestUrls()
		{
			return new string[] { IpUrl };
		}

		protected override List<SpiderProxyUriEntity> GetProxyEntities(HtmlDocument htmlDocument)
		{
			HtmlNodeCollection rows = htmlDocument.DocumentNode.SelectNodes("//table//tr");
			if (rows.IsNullOrEmpty())
			{
				return null;
			}
			var entities = new List<SpiderProxyUriEntity>(rows.Count - 1);
			for (int i = 1; i < rows.Count; i++)//Skip the columns.
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
			var tdNodes = trNode.SelectNodes("./td");
			if (tdNodes == null || tdNodes.Count < 8)
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
			byte anonymityDegree = GetAnonymityDegreeFromText(anonymityDegreeText);
			string categoryText = tdNodes[3].InnerText;
			byte category = (byte)(categoryText.Contains("HTTPS", StringComparison.CurrentCultureIgnoreCase) ? 1 : 0);
			string location = tdNodes[4].InnerText?.Trim() + ' ' + tdNodes[5].InnerText?.Trim();
			string responseTimespanText = tdNodes[6].InnerText;
			int responseMilliseconds = ParseResponseMilliseconds(responseTimespanText);
			SpiderProxyUriEntity entity = new SpiderProxyUriEntity()
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

		private static byte GetAnonymityDegreeFromText(string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				if (text.Contains("高匿"))
				{
					return 3;
				}
				if (text.Contains("普匿"))
				{
					return 1;
				}
			}
			return 0;
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