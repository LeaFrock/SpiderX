using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

		public async Task<long> SetRemoveAsync(int dbId, string keyName, IEnumerable<long> values)
		{
			var db = GetDatabase(dbId);
			long add = await db.SetRemoveAsync(keyName, values.Select(p => (RedisValue)p).ToArray());
			return add;
		}

		public async Task<bool> KeyExpireAsync(int dbId, string keyName, DateTime? dt)
		{
			var db = GetDatabase(dbId);
			return await db.KeyExpireAsync(keyName, dt);
		}

		public async Task<long> PublishAsync(string channel, string msg)
		{
			var subscriber = ConnMultiplexer.GetSubscriber();
			return await subscriber.PublishAsync(channel, msg);
		}

		public async Task SubscribeAsync(string channel, Action<RedisChannel, RedisValue> handler)
		{
			var subscriber = ConnMultiplexer.GetSubscriber();
			await subscriber.SubscribeAsync(channel, handler);
		}

		public EndPoint[] GetEndPoints()
		{
			return ConnMultiplexer.GetEndPoints();
		}

		public async Task<TimeSpan> GetRedisTimeOffset(string host, int port = 6379)
		{
			var server = ConnMultiplexer.GetServer(host, port);
			DateTime redisTime = await server.TimeAsync();
			return redisTime - DateTime.Now;
		}

		public void Disable()
		{
			ConnMultiplexer?.Dispose();
		}

		public static SpiderRedisClient Create(string connString, TextWriter log = null)
		{
			var opt = ConfigurationOptions.Parse(connString, false);
			var client = new SpiderRedisClient()
			{
				ConnMultiplexer = ConnectionMultiplexer.Connect(opt, log)
			};
			return client;
		}

		public static async Task<SpiderRedisClient> CreateAsync(string connString, TextWriter log = null)
		{
			var opt = ConfigurationOptions.Parse(connString, false);
			return await CreateAsync(opt, log);
		}

		public static async Task<SpiderRedisClient> CreateAsync(ConfigurationOptions configuration, TextWriter log = null)
		{
			var client = new SpiderRedisClient
			{
				ConnMultiplexer = await ConnectionMultiplexer.ConnectAsync(configuration, log)
			};
			return client;
		}
	}
}