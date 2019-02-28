using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SpiderX.Http;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.ProxyFetcher
{
	internal sealed class FateZeroProxyApiProvider : ProxyApiProvider
	{
		public FateZeroProxyApiProvider()
		{
			HomePageHost = "raw.githubusercontent.com";
			HomePageUrl = "https://github.com/";
		}

		public const string IpUrl = "https://raw.githubusercontent.com/fate0/proxylist/master/proxy.list";

		private readonly static Dictionary<string, string> _locationDict = new Dictionary<string, string>()
		{
			{ "CN","中国" },
			{ "HK", "中国香港" },
			{ "US", "美国"},
			{ "FR", "法国" },
			{ "CA", "加拿大"},
			{ "JP", "日本"},
		};

		public static IReadOnlyDictionary<string, string> LocationDict => _locationDict;

		public override SpiderWebClient CreateWebClient()
		{
			SpiderWebClient client = new SpiderWebClient();
			client.DefaultRequestHeaders.Host = HomePageHost;
			client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
			client.DefaultRequestHeaders.Add("Accept-Encoding", "br");
			client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
			client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
			client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
			return client;
		}

		public override string GetRequestUrl()
		{
			return IpUrl;
		}

		public override IList<string> GetRequestUrls()
		{
			return new string[] { IpUrl };
		}

		public override List<SpiderProxyUriEntity> GetProxyEntities<T>(T responseReader)
		{
			List<SpiderProxyUriEntity> entities = new List<SpiderProxyUriEntity>(512);
			using (responseReader)
			{
				string line;
				while ((line = responseReader.ReadLine()) != null)
				{
					var entity = CreateProxyEntity(line);
					if (entity != null)
					{
						entities.Add(entity);
					}
				}
			}
			return entities;
		}

		private static SpiderProxyUriEntity CreateProxyEntity(string text)
		{
			JToken item = JsonTool.DeserializeObject<JToken>(text);
			if (item == null || !item.Any())
			{
				return null;
			}
			string locationText = item.Value<string>("country")?.Trim()?.ToUpper();
			if (string.IsNullOrEmpty(locationText))
			{
				return null;
			}
			if (!LocationDict.TryGetValue(locationText, out string location))
			{
				return null;
			}
			int port = item.Value<int>("port");
			if (port < 1)
			{
				return null;
			}
			string host = item.Value<string>("host")?.Trim();
			if (string.IsNullOrEmpty(host))
			{
				return null;
			}
			double responseInterval = item.Value<double>("response_time");
			if (responseInterval < 0.001d)
			{
				responseInterval = 10d;
			}
			string categoryText = item.Value<string>("type");
			byte category = 0;
			if (categoryText != null && categoryText.Contains("https", StringComparison.CurrentCultureIgnoreCase))
			{
				category = 1;
			}
			string anonymityText = item.Value<string>("anonymity");
			byte anonymityDegree = GetAnonymityDegreeFromText(anonymityText);
			return new SpiderProxyUriEntity()
			{
				Host = host,
				Port = port,
				Category = category,
				ResponseMilliseconds = (int)(responseInterval * 1000),
				AnonymityDegree = anonymityDegree,
				Location = location
			};
		}

		private static byte GetAnonymityDegreeFromText(string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				if (text.Contains("high", StringComparison.CurrentCultureIgnoreCase))
				{
					return 3;
				}
				if (text.Contains("anony", StringComparison.CurrentCultureIgnoreCase))
				{
					return 1;
				}
			}
			return 0;
		}
	}
}