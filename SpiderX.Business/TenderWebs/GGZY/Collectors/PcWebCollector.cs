using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SpiderX.BusinessBase;
using SpiderX.Http;

namespace SpiderX.Business.TenderWebs
{
	public sealed partial class GgzyGovBll : BllBase
	{
		public sealed class PcWebCollector : CollectorBase
		{
			public override Task<List<OpenTenderEntity>> CollectOpenBids(string[] keywords)
			{
				return null;
			}

			public override Task<List<WinTenderEntity>> CollectWinBids(string[] keywords)
			{
				return null;
			}

			private static HashSet<string> GetBidUrls(string keyword)
			{
				return null;
			}

			private static bool ValidateResponseTextOK(string rspText)
			{
				return rspText.EndsWith("}") && rspText.Contains("data", StringComparison.CurrentCultureIgnoreCase);
			}

			private static SpiderHttpClient CreateWebClient(IWebProxy proxy)
			{
				var client = new SpiderHttpClient(proxy);
				client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
				client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
				client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
				client.DefaultRequestHeaders.Add("Origin", "http://deal.ggzy.gov.cn");
				client.DefaultRequestHeaders.Referrer = PcWebApiProvider.ReferUri;
				client.DefaultRequestHeaders.Host = "deal.ggzy.gov.cn";
				client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
				client.DefaultRequestHeaders.Add("Pragma", "no-cache");
				client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
				return client;
			}

			private sealed class UrlInfo
			{
				private readonly HashSet<string> _keywords = new HashSet<string>();

				public UrlInfo(string url)
				{
					Url = url;
				}

				public string Url { get; }

				public IReadOnlyCollection<string> Keywords => _keywords;

				public void AddKeyword(string word)
				{
					lock (_keywords)
					{
						_keywords.Add(word);
					}
				}

				public string PrintKeywords()
				{
					lock (_keywords)
					{
						return string.Join('/', _keywords);
					}
				}
			}
		}
	}
}