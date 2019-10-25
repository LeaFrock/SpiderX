using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	public sealed class FateZeroProxyBll : ProxyBll
	{
		public FateZeroProxyBll(ILogger logger, string[] runSetting, string dbConfigName, int version) : base(logger, runSetting, dbConfigName, version)
		{
		}

		internal override ProxyApiProvider ApiProvider => new FateZeroProxyApiProvider();

		public override async Task RunAsync()
		{
			string caseName = ClassName;
			var pa = ProxyDbContext.CreateInstance();
			string url = ApiProvider.GetRequestUrl();
			using var client = ApiProvider.CreateWebClient();
			var entities = await GetProxyEntitiesAsync(client, HttpMethod.Get, url);
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