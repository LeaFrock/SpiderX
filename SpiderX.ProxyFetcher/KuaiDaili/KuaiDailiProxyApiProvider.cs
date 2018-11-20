﻿using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using SpiderX.Http;
using SpiderX.Proxy;
using SpiderX.Tools;
using SpiderX.Extensions;

namespace SpiderX.ProxyFetcher
{
	internal class KuaiDailiProxyApiProvider : ProxyApiProvider
	{
		public KuaiDailiProxyApiProvider()
		{
			HomePageHost = "www.kuaidaili.com";
			HomePageUrl = "https://www.kuaidaili.com/";
		}

		public override List<SpiderProxyEntity> GetProxyEntities(string response)
		{
			var htmlDocument = HttpConsole.DefaultHtmlResponser.LoadHtml(response);
			if (htmlDocument == null)
			{
				return null;
			}
			HtmlNodeCollection rows = htmlDocument.DocumentNode.SelectNodes("//div[contains(@id,'content')]//div[contains(@id,'list')]/table/tbody/tr");
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