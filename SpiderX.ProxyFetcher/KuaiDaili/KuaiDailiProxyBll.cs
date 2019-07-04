using System.Net.Http;
using System.Threading.Tasks;
using SpiderX.Http;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	public sealed class KuaiDailiProxyBll : ProxyBll
	{
		internal override ProxyApiProvider ApiProvider { get; } = new KuaiDailiProxyApiProvider();

		public override async Task RunAsync()
		{
			await base.RunAsync();
			string caseName = ClassName;
			var pa = ProxyAgent<SqlServerProxyDbContext>.CreateInstance("SqlServerTest", true, c => new SqlServerProxyDbContext(c));
			var urls = ApiProvider.GetRequestUrls();
			using (SpiderWebClient webClient = ApiProvider.CreateWebClient())
			{
				var entities = GetProxyEntities(webClient, HttpMethod.Get, urls, urls.Count * 15);
				if (entities.Count < 1)
				{
					return;
				}
				entities.ForEach(e => e.Source = caseName);
				ShowConsoleMsg("CollectCount: " + entities.Count.ToString());
				int insertCount = pa.InsertProxyEntities(entities);
				ShowConsoleMsg("InsertCount: " + insertCount.ToString());
			}
		}

		public override Task RunAsync(params string[] args)
		{
			return RunAsync();
		}
	}
}