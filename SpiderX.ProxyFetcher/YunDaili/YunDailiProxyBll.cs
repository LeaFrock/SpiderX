using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.Http;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	public class YunDailiProxyBll : ProxyBll
	{
		public YunDailiProxyBll(ILogger logger, string[] runSetting, int version) : base(logger, runSetting, version)
		{
		}

		internal override ProxyApiProvider ApiProvider { get; } = new YunDailiProxyApiProvider();

		public override async Task RunAsync()
		{
			string caseName = ClassName;
			var pa = ProxyAgent<SqlServerProxyDbContext>.CreateInstance("SqlServerTest", true, c => new SqlServerProxyDbContext(c));
			var urls = ApiProvider.GetRequestUrls();
			using (SpiderWebClient webClient = ApiProvider.CreateWebClient())
			{
				var entities = GetProxyEntities(webClient, HttpMethod.Get, urls, 200);
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
	}
}