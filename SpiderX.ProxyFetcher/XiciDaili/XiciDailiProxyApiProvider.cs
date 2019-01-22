using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using SpiderX.Http;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.ProxyFetcher
{
	internal sealed class XiciDailiProxyApiProvider : ProxyApiProvider
	{
		public XiciDailiProxyApiProvider() : base()
		{
			HomePageHost = "www.xicidaili.com";
			HomePageUrl = "http://www.xicidaili.com/";
		}

		public const string NnUrlTemplate = "http://www.xicidaili.com/nn/";

		public override SpiderWebClient CreateWebClient()
		{
			SpiderWebClient client = SpiderWebClient.CreateDefault();
			client.InnerClientHandler.UseProxy = false;
			client.DefaultRequestHeaders.Host = HomePageHost;
			client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
			client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
			client.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
			client.DefaultRequestHeaders.Add("DNT", "1");
			client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
			return client;
		}

		public override List<SpiderProxyEntity> GetProxyEntities<T>(T reader)
		{
			var htmlDocument = HtmlTool.LoadFromTextReader(reader);
			if (htmlDocument == null)
			{
				return null;
			}
			HtmlNodeCollection rows = htmlDocument.DocumentNode
				.SelectNodes("//table[contains(@id,'ip')]//tr");//如果用Chrome或FireFox，浏览器会自动补全tbody，但此处XPath不能写作"//table[contains(@id,'ip')]/tbody//tr".
			if (rows == null || rows.Count < 2)
			{
				return null;
			}
			var entities = new List<SpiderProxyEntity>(rows.Count - 1);//Skip the row of ColumnNames.
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

		private static SpiderProxyEntity CreateProxyEntity(HtmlNode node)
		{
			HtmlNodeCollection tds = node.SelectNodes("./td");
			if (tds == null || tds.Count < 10)
			{
				return null;
			}
			HtmlNode lifeTimeNode = tds[8];
			string lifeTimeText = lifeTimeNode.InnerText;
			if (string.IsNullOrEmpty(lifeTimeText))
			{
				return null;
			}
			if (!lifeTimeText.Contains("天") || !StringTool.TryMatchDouble(lifeTimeText, out double lifeTime) || lifeTime < 5)//Only create proxies with 5d+ alive.
			{
				return null;
			}
			HtmlNode portNode = tds[2];
			string portText = portNode.InnerText.Trim();
			if (!int.TryParse(portText, out int port))
			{
				return null;
			}
			double responseSeconds = GetSpeedSeconds(tds[6]);
			if (responseSeconds > 2.5d)
			{
				return null;
			}
			double requestSeconds = GetSpeedSeconds(tds[7]);
			if (requestSeconds > 2.5d)
			{
				return null;
			}
			HtmlNode ipNode = tds[1];
			string host = ipNode.InnerText.Trim();
			HtmlNode locationNode = tds[3];
			string location = locationNode.InnerText?.Trim();
			HtmlNode categoryNode = tds[5];
			string categoryText = categoryNode.InnerText;
			byte category = (byte)((categoryText != null && categoryText.Contains("HTTPS", StringComparison.CurrentCultureIgnoreCase)) ? 1 : 0);
			int responseMilliseconds = (int)(1000 * (requestSeconds + responseSeconds));
			return new SpiderProxyEntity()
			{
				Host = host,
				Port = port,
				AnonymityDegree = 3,
				Category = category,
				Location = location,
				ResponseMilliseconds = responseMilliseconds
			};
		}

		private static double GetSpeedSeconds(HtmlNode node)
		{
			var divNode = node.SelectSingleNode("./div[@title]");
			string nodeText = divNode?.GetAttributeValue("title", null);
			if (nodeText == null)
			{
				return 1000000f;
			}
			if (!StringTool.TryMatchDouble(nodeText, out double result))
			{
				return 1000000f;
			}
			return result;
		}
	}
}