using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.BusinessBase;
using SpiderX.Extensions;
using SpiderX.Extensions.Http;
using SpiderX.Http;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.ProxyFetcher
{
	public abstract class ProxyBll : BllBase
	{
		public ProxyBll(ILogger logger, string[] runSetting, int version) : base(logger, runSetting, version)
		{
		}

		internal abstract ProxyApiProvider ApiProvider { get; }

		protected async Task<List<SpiderProxyUriEntity>> GetProxyEntitiesAsync(SpiderWebClient webClient, HttpMethod httpMethod, string url)
		{
			List<SpiderProxyUriEntity> entities = null;
			var responseMessage = await webClient.SendAsync(httpMethod, url);
			if (responseMessage != null)
			{
				using (responseMessage)
				{
					if (responseMessage.IsSuccessStatusCode)
					{
						var reader = await responseMessage.Content.ToHtmlReaderAsync();
						entities = ApiProvider.GetProxyEntities(reader);
					}
				};
			}
			return entities ?? new List<SpiderProxyUriEntity>(0);
		}

		protected async Task<List<SpiderProxyUriEntity>> GetProxyEntitiesAsync(SpiderWebClient webClient, HttpMethod httpMethod, IList<string> urls, int estimatedCount = 0)
		{
			var entities = estimatedCount > 0 ? new List<SpiderProxyUriEntity>(estimatedCount) : new List<SpiderProxyUriEntity>();
			int urlCount = urls.Count;
			Task[] tasks = new Task[urlCount];
			for (int i = 0; i < urlCount; i++)
			{
				tasks[i] = Task.Run(async () =>
				{
					var tempList = await GetProxyEntitiesAsync(webClient, httpMethod, urls[i]);
					if (!tempList.IsNullOrEmpty())
					{
						lock (entities)
						{
							entities.AddRange(tempList);
						}
					}
				});
				await Task.Delay(RandomTool.NextIntSafely(4000, 6000));
			}
			await Task.WhenAll(tasks);
			return entities;
		}
	}
}