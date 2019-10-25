using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	public sealed class KuaiDailiProxyBll : ProxyBll
	{
		public KuaiDailiProxyBll(ILogger logger, string[] runSetting, string dbConfigName, int version) : base(logger, runSetting, dbConfigName, version)
		{
		}

		internal override ProxyApiProvider ApiProvider { get; } = new KuaiDailiProxyApiProvider();

		public override async Task RunAsync()
		{
			string caseName = ClassName;
			using var pa = ProxyDbContext.CreateInstance();
			var urls = ApiProvider.GetRequestUrls();
			using var webClient = ApiProvider.CreateWebClient();
			var entities = await GetProxyEntitiesAsync(webClient, HttpMethod.Get, urls, urls.Count * 15);
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