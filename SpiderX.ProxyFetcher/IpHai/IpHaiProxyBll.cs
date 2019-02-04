using System.Net.Http;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	public sealed class IpHaiProxyBll : ProxyBll
	{
		internal override ProxyApiProvider ApiProvider { get; } = new IpHaiProxyApiProvider();

		public override void Run()
		{
			base.Run();
			string caseName = ClassName;
			var pa = ProxyAgent<SqlServerProxyDbContext>.CreateInstance("SqlServerTest", true, c => new SqlServerProxyDbContext(c));
			var urls = ApiProvider.GetRequestUrls();
			using (var client = ApiProvider.CreateWebClient())
			{
				var entities = GetProxyEntities(client, HttpMethod.Get, urls, 64);
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

		public override void Run(params string[] args)
		{
			Run();
		}
	}
}