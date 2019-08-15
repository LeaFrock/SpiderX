using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpiderX.Proxy;
using SpiderX.ProxyFilter.Events;
using SpiderX.Redis;
using StackExchange.Redis;

namespace SpiderX.ProxyFilter
{
	internal sealed class RedisProxyReceiver : IProxyReceiver
	{
		private readonly string _connStr;
		private readonly string _channel;

		public RedisProxyReceiver(string connStr, string channel)
		{
			_connStr = connStr;
			_channel = channel;
		}

		private SpiderRedisClient _client;

		public EventHandler<ProxyReceiveEventArgs> OnProxyReceived { get; set; }

		public async Task InitializeAsync()
		{
			var confOpts = ConfigurationOptions.Parse(_connStr);
			_client = await SpiderRedisClient.CreateAsync(confOpts);
			await _client.SubscribeAsync(_channel, DeserializeProxies);
		}

		public void Disable()
		{
			_client.Disable();
		}

		private void DeserializeProxies(RedisChannel redisChannel, RedisValue redisValue)
		{
			if (!redisValue.HasValue || redisValue.IsNullOrEmpty)
			{
				OnProxyReceived?.Invoke(this, ProxyReceiveEventArgs.Empty);
				return;
			}
			string json = redisValue.ToString();
			var proxies = JsonConvert.DeserializeObject<SpiderProxyUriEntity[]>(json);
			OnProxyReceived?.Invoke(this, new ProxyReceiveEventArgs() { Proxies = proxies });
		}
	}
}