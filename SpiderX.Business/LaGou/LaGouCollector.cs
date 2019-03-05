using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using SpiderX.Http;
using SpiderX.Http.Util;
using SpiderX.Proxy;

namespace SpiderX.Business.LaGou
{
	public sealed partial class LaGouBll
	{
		private class PcWebCollector : CollectorBase
		{
			public override void Collect()
			{
				throw new NotImplementedException();
			}

			private SpiderWebClient CreateWebClient()
			{
				var uris = GetUrisFromDb();
				var webProxy = CreateWebProxy(uris);

				SpiderWebClient client = new SpiderWebClient(webProxy);
				client.DefaultRequestHeaders.Host = PcWebApiProvider.HomePageHost;
				client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
				client.DefaultRequestHeaders.Add("Accept-Encoding", "br");
				client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
				client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
				client.DefaultRequestHeaders.Add("User-Agent", HttpConsole.DefaultPcUserAgent);
				client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
				client.DefaultRequestHeaders.Add("X-Anit-Forge-Code", "0");
				client.DefaultRequestHeaders.Add("X-Anit-Forge-Token", "None");
				client.DefaultRequestHeaders.Add("Origin", PcWebApiProvider.HomePageUrl);
				return client;
			}

			private static IWebProxy CreateWebProxy(IEnumerable<Uri> proxies)
			{
				DefaultSpiderProxyUriSelector uriSelector = new DefaultSpiderProxyUriSelector();
				uriSelector.Init(proxies);
				return new SpiderWebProxy(uriSelector);
			}
		}

		private abstract class CollectorBase
		{
			public abstract void Collect();

			protected static IEnumerable<Uri> GetUrisFromDb()
			{
				var pa = ProxyAgent<SqlServerProxyDbContext>.CreateInstance("SqlServerTest", true, c => new SqlServerProxyDbContext(c));
				var proxies = pa.SelectProxyEntities(e => e.Category == 1, 360).Select(e => e.Value);
				return proxies;
			}
		}
	}
}