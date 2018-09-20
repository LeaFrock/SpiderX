using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SpiderX.Http;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher.KuaiDaili
{
	public sealed class KuaiDailiProxyBll : ProxyBll
	{
		public override string ClassName => GetType().Name;

		public const string HomePageHost = "www.kuaidaili.com";
		public const string HomePageUrl = "https://www.kuaidaili.com/";

		public const string InhaUrlTemplate = "https://www.kuaidaili.com/free/inha/";//国内高匿（High Anonimity）
		public const string IntrUrlTemplate = "https://www.kuaidaili.com/free/intr/";//国内透明（Transparent）

		public override void Run(params string[] args)
		{
			Run();
		}

		public const string TableItemXpath = "//div[contains(@id,'content')]//div[contains(@id,'list')]/table/tbody/tr";

		public override void Run()
		{
			base.Run();
			ProxyAgent pa = CreateProxyAgent();
			var entities = GetProxyEntities(InhaUrlTemplate, 1);
			int insertCount = pa.InsertProxyEntities(entities);
		}

		private List<SpiderProxyEntity> GetProxyEntitiesConcurrently(string urlTemplate, int maxPage)
		{
			List<SpiderProxyEntity> entities = new List<SpiderProxyEntity>(maxPage * 15);
			Parallel.For(1, maxPage + 1, new ParallelOptions() { MaxDegreeOfParallelism = Math.Min(maxPage, 50) },
				p =>
				{
					var tmpList = GetProxyEntitiesByPage(urlTemplate, p);
					if (tmpList == null || tmpList.Count < 1)
					{
						return;
					}
					lock (entities)
					{
						entities.AddRange(tmpList);
					}
				});
			return entities;
		}

		private List<SpiderProxyEntity> GetProxyEntities(string urlTemplate, int maxPage)
		{
			List<SpiderProxyEntity> entities = new List<SpiderProxyEntity>(maxPage * 15);
			for (int p = 1; p <= maxPage; p++)
			{
				var tmpList = GetProxyEntitiesByPage(urlTemplate, p);
				entities.AddRange(tmpList);
			}
			return entities;
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
			if (rows == null)
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

		private SpiderProxyEntity CreateProxyEntity(HtmlNode node)
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
			int anonymityDegree = (anonymityNode != null && anonymityText.Contains("高匿")) ? 3 : 0;
			HtmlNode categoryNode = node.SelectSingleNode("./td[contains(@data-title,'类型')]");
			string categoryText = categoryNode?.InnerText;
			int category = (categoryText != null && categoryText.Contains("Https", StringComparison.CurrentCultureIgnoreCase)) ? 1 : 0;
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

		private HttpWebRequest CreateRequest(string urlTemplate, int page)
		{
			var request = WebRequest.CreateHttp(InhaUrlTemplate + page.ToString());
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
			request.Host = HomePageHost;
			request.UserAgent = HttpConsole.DefaultPcUserAgent;
			request.Referer = GetRefererUrl(urlTemplate, page);
			request.Timeout = 5000;
			return request;
		}

		private static string GetRefererUrl(string urlTemplate, int page)
		{
			return urlTemplate + Math.Max(1, page - _randomEvent.Next(1, 5)).ToString();
		}

		private static int ParseResponseMilliseconds(string text)
		{
			if (!MatchDoubleFromText(text, out double result))
			{
				return 10000;
			}
			return (int)(result * 1000);
		}
	}
}