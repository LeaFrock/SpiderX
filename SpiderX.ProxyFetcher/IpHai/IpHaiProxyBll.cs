using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	public sealed class IpHaiProxyBll : ProxyBll
	{
		public IpHaiProxyBll(ILogger logger, string[] runSetting, int version) : base(logger, runSetting, version)
		{
		}

		internal override ProxyApiProvider ApiProvider { get; } = new IpHaiProxyApiProvider();

		public override async Task RunAsync()
		{
			string caseName = ClassName;
			using var pa = ProxyDbContext.CreateInstance();
			var urls = ApiProvider.GetRequestUrls();
			using var client = ApiProvider.CreateWebClient();
			var entities = await GetProxyEntitiesAsync(client, HttpMethod.Get, urls, urls.Count * 32);
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