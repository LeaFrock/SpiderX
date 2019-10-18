using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	public sealed class FeiyiProxyBll : ProxyBll
	{
		public FeiyiProxyBll(ILogger logger, string[] runSetting, int version) : base(logger, runSetting, version)
		{
		}

		internal override ProxyApiProvider ApiProvider => new FeiyiProxyApiProvider();

		public override async Task RunAsync()
		{
			string caseName = ClassName;
			using var pa = ProxyDbContext.CreateInstance();
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