using System.IO;
using StackExchange.Redis;

namespace SpiderX.Redis
{
	public sealed class SpiderRedisClient
	{
		private SpiderRedisClient()
		{
		}

		public ConnectionMultiplexer ConnectionMultiplexer { get; private set; }

		public static SpiderRedisClient Create(ConfigurationOptions configuration, TextWriter log = null)
		{
			var client = new SpiderRedisClient();
			client.ConnectionMultiplexer = ConnectionMultiplexer.Connect(configuration, log);
			return client;
		}

		public static SpiderRedisClient Create()
		{
			var client = new SpiderRedisClient
			{
				ConnectionMultiplexer = ConnectionMultiplexer.Connect("localhost")
			};
			return client;
		}
	}
}