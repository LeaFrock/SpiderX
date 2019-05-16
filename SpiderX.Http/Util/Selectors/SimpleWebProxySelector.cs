using System;
using System.Collections.Concurrent;
using System.Net;
using SpiderX.Proxy;

namespace SpiderX.Http.Util
{
	public sealed class SimpleWebProxySelector : IWebProxySelector
	{
		public SimpleWebProxySelector(IProxyUriLoader uriLoader)
		{
			_proxyUriLoader = uriLoader ?? throw new ArgumentNullException(nameof(_proxyUriLoader));
		}

		private readonly object _syncRoot = new object();

		private readonly IProxyUriLoader _proxyUriLoader;

		private readonly ConcurrentQueue<WebProxy> _normalQueue = new ConcurrentQueue<WebProxy>();
		private readonly ConcurrentQueue<WebProxy> _advancedQueue = new ConcurrentQueue<WebProxy>();

		private readonly ConcurrentDictionary<string, byte> _failTimesRecordOfNormalProxies = new ConcurrentDictionary<string, byte>();
		private readonly ConcurrentDictionary<string, byte> _failTimesRecordOfAdvancedProxies = new ConcurrentDictionary<string, byte>();

		public int AdvancedProxiesCacheCapacity { get; set; } = 100;

		public byte MaxFailTimesOfNormalProxies { get; set; } = 3;

		public byte MaxFailTimesOfAdvancedProxies { get; set; } = 10;

		public void OnAdvancedProxyFail(WebProxy proxy)
		{
			int currentFailTimes = _failTimesRecordOfAdvancedProxies.AddOrUpdate(proxy.Address.AbsoluteUri, 1, (k, v) => (byte)(v + 1));
			if (currentFailTimes < MaxFailTimesOfAdvancedProxies)
			{
				_advancedQueue.Enqueue(proxy);
			}
			else
			{
				_failTimesRecordOfAdvancedProxies.TryRemove(proxy.Address.AbsoluteUri, out var _);
			}
		}

		public void OnAdvancedProxySuccess(WebProxy proxy)
		{
			_advancedQueue.Enqueue(proxy);
		}

		public void OnNormalProxyFail(WebProxy proxy)
		{
			int currentFailTimes = _failTimesRecordOfNormalProxies.AddOrUpdate(proxy.Address.AbsoluteUri, 1, (k, v) => (byte)(v + 1));
			if (currentFailTimes < MaxFailTimesOfNormalProxies)
			{
				_normalQueue.Enqueue(proxy);
			}
			else
			{
				_failTimesRecordOfNormalProxies.TryRemove(proxy.Address.AbsoluteUri, out var _);
			}
		}

		public void OnNormalProxySuccess(WebProxy proxy)
		{
			if (_advancedQueue.Count < AdvancedProxiesCacheCapacity)
			{
				_advancedQueue.Enqueue(proxy);
			}
			else
			{
				_normalQueue.Enqueue(proxy);
			}
		}

		public WebProxy SelectNextProxy()
		{
			while (true)
			{
				if (_normalQueue.TryDequeue(out var proxy))
				{
					return proxy;
				}
				lock (_syncRoot)
				{
					if (_normalQueue.IsEmpty)
					{
						LoadProxies(10000);
					}
				}
			}
		}

		public bool TryPreferAdvancedProxy(out WebProxy proxy)
		{
			while (true)
			{
				if (_advancedQueue.IsEmpty)
				{
					proxy = SelectNextProxy();
					return false;
				}
				if (!_advancedQueue.TryDequeue(out proxy))
				{
					proxy = SelectNextProxy();
					return false;
				}
				return true;
			}
		}

		private void LoadProxies(int maxCount = 0)
		{
			var uris = _proxyUriLoader.Load(maxCount);
			foreach (var uri in uris)
			{
				_normalQueue.Enqueue(new WebProxy(uri));
			}
		}
	}
}