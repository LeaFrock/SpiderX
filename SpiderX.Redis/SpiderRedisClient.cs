using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace SpiderX.Redis
{
	public sealed class SpiderRedisClient
	{
		private SpiderRedisClient()
		{
		}

		internal ConnectionMultiplexer ConnMultiplexer { get; private set; }

		public IDatabase GetDatabase(int db = -1, object asyncState = null) => ConnMultiplexer.GetDatabase(db, asyncState);

		public async Task<long> SetAddAsync(int dbId, string keyName, IEnumerable<long> values)
		{
			var db = GetDatabase(dbId);
			long add = await db.SetAddAsync(keyName, values.Select(p => (RedisValue)p).ToArray());
			return add;
		}

		public async Task<bool> KeyExpireAsync(int dbId, string keyName, DateTime? dt)
		{
			var db = GetDatabase(dbId);
			return await db.KeyExpireAsync(keyName, dt);
		}

		public async Task<TimeSpan> GetRedisTimeOffset()
		{
			var server = ConnMultiplexer.GetServer("localhost", 6379);
			DateTime redisTime = await server.TimeAsync();
			return redisTime - DateTime.Now;
		}

		public void Disable()
		{
			ConnMultiplexer?.Dispose();
		}

		public static SpiderRedisClient Create(ConfigurationOptions configuration, TextWriter log = null)
		{
			var client = new SpiderRedisClient
			{
				ConnMultiplexer = ConnectionMultiplexer.Connect(configuration, log)
			};
			return client;
		}

		public static SpiderRedisClient Create()
		{
			var client = new SpiderRedisClient
			{
				ConnMultiplexer = ConnectionMultiplexer.Connect("localhost")
			};
			return client;
		}
	}
}