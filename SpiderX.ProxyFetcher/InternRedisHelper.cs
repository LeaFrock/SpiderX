using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpiderX.DataClient;
using SpiderX.Proxy;
using SpiderX.Redis;

namespace SpiderX.ProxyFetcher
{
	internal static class InternRedisHelper
	{
		private static ConcurrentDictionary<string, SpiderRedisClient> _redisClientCache = new ConcurrentDictionary<string, SpiderRedisClient>();

		internal static async Task<long> PublishProxiesAsync(IEnumerable<SpiderProxyUriEntity> proxies, DbConfig redisConfig, string redisChannel = "SpiderProxyFetcher", bool useCache = true)
		{
			string connStr = redisConfig.ConnectionString;
			SpiderRedisClient redisClient = useCache ?
				_redisClientCache.GetOrAdd(connStr, str => SpiderRedisClient.Create(str))
				: await SpiderRedisClient.CreateAsync(connStr).ConfigureAwait(false);
			string json = JsonConvert.SerializeObject(proxies);
			return await redisClient.PublishAsync(redisChannel, json);
		}

		internal static void ClearClientCache()
		{
			foreach (var c in _redisClientCache.Values)
			{
				c.Disable();
			}
			_redisClientCache.Clear();
		}
	}
}