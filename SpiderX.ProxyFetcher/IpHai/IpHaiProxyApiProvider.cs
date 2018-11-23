using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using SpiderX.Http;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.ProxyFetcher
{
	internal sealed class IpHaiProxyApiProvider : ProxyApiProvider
	{
		public IpHaiProxyApiProvider() : base()
		{
			HomePageHost = "www.iphai.com";
			HomePageUrl = "http://www.iphai.com/";
		}

		public const string NgUrl = "http://www.iphai.com/free/ng";//国内高匿
		public const string WgUrl = "http://www.iphai.com/free/wg";//国外高匿

		public override SpiderWebClient CreateWebClient()
		{
			SpiderWebClient client = SpiderWebClient.CreateDefault();
			client.InnerClientHandler.UseProxy = false;
			client.DefaultRequestHeaders.Host = HomePageHost;
			client.DefaultRequestHeaders.Referrer = new Uri(NgUrl);
			client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
			client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
			client.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
			client.DefaultRequestHeaders.Add("DNT", "1");
			client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
			return client;
		}

		public override List<SpiderProxyEntity> GetProxyEntities<T>(T responseReader)
		{
			var htmlDocument = HtmlTool.LoadFromTextReader(responseReader);
			if (htmlDocument == null)
			{
				return null;
			}
			HtmlNodeCollection rows = htmlDocument.DocumentNode.SelectNodes("//table[contains(@class,'table')]/tr");
			if (rows == null || rows.Count <= 1)
			{
				return null;
			}
			var entities = new List<SpiderProxyEntity>(rows.Count - 1);
			for (int i = 1; i < rows.Count; i++)//Skip the ColumnName row.
			{
				var entity = CreateProxyEntity(rows[i]);
				if (entity != null)
				{
					entities.Add(entity);
				}
			}
			return entities;
		}

		private static SpiderProxyEntity CreateProxyEntity(HtmlNode trNode)
		{
			var tdNodes = trNode.SelectNodes("./td");
			if (tdNodes == null || tdNodes.Count < 6)
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
			byte anonymityDegree = (byte)(anonymityDegreeText.Contains("高") ? 3 : 1);
			string categoryText = tdNodes[3].InnerText;
			byte category = (byte)(categoryText.Contains("HTTPS", StringComparison.CurrentCultureIgnoreCase) ? 1 : 0);
			string location = tdNodes[4].InnerText.Trim();
			string responseTimespanText = tdNodes[5].InnerText;
			int responseMilliseconds = ParseResponseMilliseconds(responseTimespanText);
			//if(...)return null;
			SpiderProxyEntity entity = new SpiderProxyEntity()
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