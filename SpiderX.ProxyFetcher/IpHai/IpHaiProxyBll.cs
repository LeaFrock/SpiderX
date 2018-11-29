using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SpiderX.Extensions;
using SpiderX.Extensions.Http;
using SpiderX.Http;
using SpiderX.Proxy;

namespace SpiderX.ProxyFetcher
{
	public sealed class IpHaiProxyBll : ProxyBll
	{
		internal override ProxyApiProvider ApiProvider { get; } = new IpHaiProxyApiProvider();

		public override void Run(params string[] args)
		{
			Run();
		}

		public override void Run()
		{
			base.Run();
			string caseName = ClassName;
			var pa = ProxyAgent<SqlServerProxyDbContext>.CreateInstance("SqlServerTest", true, c => new SqlServerProxyDbContext(c));
			using (var client = ApiProvider.CreateWebClient())
			{
				var entities = GetProxyEntities(client, IpHaiProxyApiProvider.NgUrl, IpHaiProxyApiProvider.WgUrl);
				if (entities.Count < 1)
				{
					return;
				}
				entities.ForEach(e => e.Source = caseName);
				ShowDebugInfo("CollectCount: " + entities.Count.ToString());
				int insertCount = pa.InsertProxyEntities(entities);
				ShowDebugInfo("InsertCount: " + insertCount.ToString());
			}
		}

		private List<SpiderProxyEntity> GetProxyEntities(SpiderWebClient webClient, params string[] urls)
		{
			var entities = new List<SpiderProxyEntity>(64);
			Task[] tasks = new Task[urls.Length];
			int index = 0;
			foreach (string url in urls)
			{
				tasks[index] = webClient.GetAsync(url)
					.ContinueWith(httpTask =>
					{
						var responseMessage = httpTask.Result;
						responseMessage.Content.ToStreamReaderAsync()
						.ContinueWith(streamTask =>
						{
							StreamReader reader = streamTask.Result;
							var tempList = ApiProvider.GetProxyEntities(reader);
							if (!tempList.IsNullOrEmpty())
							{
								lock (entities)
								{
									entities.AddRange(tempList);
								}
							}
						});
					});
				index++;
				Thread.Sleep(RandomEvent.Next(4000, 6000));
			}
			try
			{
				Task.WaitAll(tasks);
			}
			catch
			{
			}
			return entities;
		}
	}
}