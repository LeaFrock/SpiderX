using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpiderX.Proxy;
using SpiderX.Redis;
using StackExchange.Redis;

namespace SpiderX.ProxyFilter
{
	internal class Program
	{
		private readonly static ConcurrentBag<SpiderProxyUriEntity> _proxyBag = new ConcurrentBag<SpiderProxyUriEntity>();

		private static async Task Main(string[] args)
		{
			SpiderRedisClient client;
			if (args != null && args.Length > 1 && !string.IsNullOrEmpty(args[0]))
			{
				string connStr = args[0];
				client = await SpiderRedisClient.CreateAsync(connStr);
			}
			else
			{
				string connStr = Console.ReadLine();
				ConfigurationOptions confOpts;
				while (!CheckRedisConnString(connStr, out confOpts))
				{
					Console.WriteLine("Invalid Redis ConnectionString. Input again please.");
					connStr = Console.ReadLine();
				}
				client = await SpiderRedisClient.CreateAsync(confOpts);
			}
			Console.WriteLine("RedisClient Initialized.");
			await client.SubscribeAsync("SpiderProxyFetcher", DeserializeProxies);
			Console.WriteLine("RedisClient Subscribed: " + "SpiderProxyFetcher");
			Console.WriteLine("Press Key to prepare for exiting.");
			Console.ReadKey();
			client.Disable();
			Console.WriteLine("Release the RedisClient. Press Key again and exit!");
			Console.ReadKey();
		}

		private static bool CheckRedisConnString(string connStr, out ConfigurationOptions confOpts)
		{
			if (string.IsNullOrWhiteSpace(connStr))
			{
				confOpts = null;
				return false;
			}
			try
			{
				confOpts = ConfigurationOptions.Parse(connStr);
			}
			catch
			{
				confOpts = null;
				return false;
			}
			return true;
		}

		private static void DeserializeProxies(RedisChannel redisChannel, RedisValue redisValue)
		{
			if (!redisValue.HasValue || redisValue.IsNullOrEmpty)
			{
				Console.WriteLine($"[{DateTime.Now.ToString("MM/dd-hh:mm:ss")}][AddProxies] Empty.");
				return;
			}
			string json = redisValue.ToString();
			var proxies = JsonConvert.DeserializeObject<SpiderProxyUriEntity[]>(json);
			foreach (var proxy in proxies)
			{
				_proxyBag.Add(proxy);
			}
			Console.WriteLine($"[{DateTime.Now.ToString("MM/dd-hh:mm:ss")}][AddProxies] {proxies.Length.ToString()}.");
		}
	}
}