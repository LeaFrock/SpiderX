using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SpiderX.BusinessBase;
using SpiderX.DataClient;
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

		protected virtual async Task<long> PublishProxiesToRedisAsync(IEnumerable<SpiderProxyUriEntity> entities, string dbConfigName, bool isTest)
		{
			var redisConfig = DbConfigManager.Default.GetConfig(dbConfigName, isTest);
			if (redisConfig == null)
			{
				ShowLogError($"DbConfig[{dbConfigName}] Not Found.");
				return -1;
			}
			long result = await InternRedisHelper.PublishProxiesAsync(entities, redisConfig, useCache: false);
			ShowLogInfo(ClassName + "_PublishProxiesAsync: " + result.ToString());
			return result;
		}

		protected async Task<List<SpiderProxyUriEntity>> GetProxyEntitiesAsync(SpiderHttpClient webClient, HttpMethod httpMethod, string url)
		{
			List<SpiderProxyUriEntity> entities = null;
			using (var reqMsg = new HttpRequestMessage(httpMethod, url))
			{
				HttpResponseMessage rspMsg;
				try
				{
					rspMsg = await webClient.SendAsync(reqMsg);
				}
				catch
				{
					return new List<SpiderProxyUriEntity>(1);
				}
				if (rspMsg != null)
				{
					using (rspMsg)
					{
						if (rspMsg.IsSuccessStatusCode)
						{
							var reader = await rspMsg.Content.ToHtmlReaderAsync();
							entities = ApiProvider.GetProxyEntities(reader);
						}
					};
				}
			}
			return entities ?? new List<SpiderProxyUriEntity>(1);
		}

		protected async Task<List<SpiderProxyUriEntity>> GetProxyEntitiesAsync(SpiderHttpClient webClient, HttpMethod httpMethod, IList<string> urls, int estimatedCount = 0)
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