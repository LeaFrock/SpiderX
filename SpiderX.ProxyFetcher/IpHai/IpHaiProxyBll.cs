using System;
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
			ProxyAgent pa = CreateProxyAgent();
			using (var client = ApiProvider.CreateWebClient())
			{
				var entities = GetProxyEntities(client, IpHaiProxyApiProvider.NgUrl, IpHaiProxyApiProvider.WgUrl);
				entities.ForEach(e => e.Source = caseName);
				int insertCount = pa.InsertProxyEntities(entities);
				Console.WriteLine(insertCount.ToString());
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
						responseMessage.ToStreamAsync()
						.ContinueWith(streamTask =>
						{
							Stream stream = streamTask.Result;
							var tempList = ApiProvider.GetProxyEntities(stream);
							if (!tempList.IsNullOrEmpty())
							{
								lock (entities)
								{
									entities.AddRange(tempList);
								}
							}
						});
					});
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