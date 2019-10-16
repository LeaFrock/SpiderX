using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	public class KxDailiProxyBll : ProxyBll
	{
		public KxDailiProxyBll(ILogger logger, string[] runSetting, int version) : base(logger, runSetting, version)
		{
		}

		internal override ProxyApiProvider ApiProvider { get; } = new KxDailiProxyApiProvider();

		public override async Task RunAsync()
		{
			string caseName = ClassName;
			using var pa = ProxyDbContext.CreateInstance("SqlServerTest");
			var urls = ApiProvider.GetRequestUrls();
			using var webClient = ApiProvider.CreateWebClient();
			var entities = await GetProxyEntitiesAsync(webClient, HttpMethod.Get, urls, urls.Count * 10);
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