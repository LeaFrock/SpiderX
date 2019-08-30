﻿using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.Http;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	public sealed class SixSixIpCNProxyBll : ProxyBll
	{
		public SixSixIpCNProxyBll(ILogger logger, string[] runSetting, int version) : base(logger, runSetting, version)
		{
		}

		internal override ProxyApiProvider ApiProvider { get; } = new SixSixIpCNProxyApiProvider();

		public async override Task RunAsync()
		{
			string caseName = ClassName;
			var pa = ProxyAgent<SqlServerProxyDbContext>.CreateInstance("SqlServerTest", true, c => new SqlServerProxyDbContext(c));
			var urls = ApiProvider.GetRequestUrls();
			using (SpiderHttpClient webClient = ApiProvider.CreateWebClient())
			{
				var entities = await GetProxyEntitiesAsync(webClient, HttpMethod.Get, urls, urls.Count * 64);
				if (entities.Count < 1)
				{
					return;
				}
				entities.ForEach(e => e.Source = caseName);
				ShowLogInfo("CollectCount: " + entities.Count.ToString());
				int insertCount = pa.InsertProxyEntities(entities);
				ShowLogInfo("InsertCount: " + insertCount.ToString());
			}
		}
	}
}