using System;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;
using SpiderX.Http;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher.XiciDaili
{
	public sealed class XiciDailiProxyBll : ProxyBll
	{
		public override string ClassName => GetType().Name;

		public const string HomePageHost = "www.xicidaili.com";

		public const string UrlTemplate = "http://www.xicidaili.com/nn/";

		public override void Run(params string[] args)
		{
			Run();
		}

		public const string TableItemXpath = "//table[contains(@id,'ip')]/tbody//tr";

		public override void Run()
		{
			base.Run();
			var aaa = GetProxyEntitiesByPage(UrlTemplate, 1);
		}

		private List<SpiderProxyEntity> GetProxyEntitiesByPage(string urlTemplate, int page)
		{
			var request = CreateRequest(urlTemplate, page);
			var response = request.GetResponse();
			if (response == null)
			{
				return null;
			}
			var htmlDocument = _defaultHtmlResponser.LoadHtml(response);
			if (htmlDocument == null)
			{
				return null;
			}
			HtmlNodeCollection rows = htmlDocument.DocumentNode.SelectNodes(TableItemXpath);
			if (rows == null || rows.Count < 2)
			{
				return null;
			}
			var entities = new List<SpiderProxyEntity>(rows.Count - 1);
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

		private SpiderProxyEntity CreateProxyEntity(HtmlNode node)
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
			if (!lifeTimeText.Contains("天") || !MatchDoubleFromText(lifeTimeText, out double lifeTime) || lifeTime < 45)//只保留45+天的Proxy
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
			int category = (categoryText != null && categoryText.Contains("HTTPS", StringComparison.CurrentCultureIgnoreCase)) ? 1 : 0;
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

		private HttpWebRequest CreateRequest(string urlTemplate, int page)
		{
			var request = WebRequest.CreateHttp(urlTemplate + page.ToString());
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
			request.Host = HomePageHost;
			request.UserAgent = HttpConsole.DefaultPcUserAgent;
			request.Referer = GetRefererUrl(urlTemplate, page);
			request.Timeout = 5000;
			return request;
		}

		private static double GetSpeedSeconds(HtmlNode node)
		{
			var divNode = node.SelectSingleNode("./div[@title]");
			string nodeText = divNode?.GetAttributeValue("title", null);
			if (nodeText == null)
			{
				return 1000000f;
			}
			if (!MatchDoubleFromText(nodeText, out double result))
			{
				return 1000000f;
			}
			return result;
		}

		private static string GetRefererUrl(string urlTemplate, int page)
		{
			return urlTemplate + Math.Max(1, page - _randomEvent.Next(1, 5)).ToString();
		}
	}
}