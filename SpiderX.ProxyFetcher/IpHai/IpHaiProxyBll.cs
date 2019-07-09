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
			var pa = ProxyAgent<SqlServerProxyDbContext>.CreateInstance("SqlServerTest", true, c => new SqlServerProxyDbContext(c));
			var urls = ApiProvider.GetRequestUrls();
			using (var client = ApiProvider.CreateWebClient())
			{
				var entities = GetProxyEntities(client, HttpMethod.Get, urls, urls.Count * 32);
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