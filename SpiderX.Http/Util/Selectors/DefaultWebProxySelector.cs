using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SpiderX.Proxy;

namespace SpiderX.Http.Util
{
	public sealed class DefaultWebProxySelector : IWebProxySelector
	{
		public DefaultWebProxySelector(Uri uri, IProxyUriLoader uriLoader, IWebProxyValidator proxyValidator)
		{
			TargetUri = uri ?? throw new ArgumentNullException(nameof(TargetUri));
			_proxyUriLoader = uriLoader ?? throw new ArgumentNullException(nameof(_proxyUriLoader));
			_proxyValidator = proxyValidator ?? throw new ArgumentNullException(nameof(_proxyValidator));
			ProxyValidatorConfig = _proxyValidator.Config;
		}

		private readonly IProxyUriLoader _proxyUriLoader;
		private readonly IWebProxyValidator _proxyValidator;

		private readonly ConcurrentQueue<WebProxy> _verifyingQueue = new ConcurrentQueue<WebProxy>();
		private readonly ConcurrentQueue<WebProxy> _normalQueue = new ConcurrentQueue<WebProxy>();
		private readonly ConcurrentQueue<WebProxy> _advancedQueue = new ConcurrentQueue<WebProxy>();

		private readonly ConcurrentDictionary<string, byte> _failTimesRecordOfNormalProxies = new ConcurrentDictionary<string, byte>();
		private readonly ConcurrentDictionary<string, byte> _failTimesRecordOfAdvancedProxies = new ConcurrentDictionary<string, byte>();

		private int _initCode;

		public bool IsInited => _initCode != 0;

		public WebProxyValidatorConfig ProxyValidatorConfig { get; }

		public int AdvancedProxiesCacheCapacity { get; set; } = 100;

		public byte MaxFailTimesOfNormalProxies { get; set; } = 3;

		public byte MaxFailTimesOfAdvancedProxies { get; set; } = 10;

		public Uri TargetUri { get; }

		public void Initialize()
		{
			if (Interlocked.CompareExchange(ref _initCode, 1, 0) == 0)
			{
				LoadProxies();
				Task.Factory.StartNew(VerifyProxies, TaskCreationOptions.LongRunning);
			}
		}

		public WebProxy SelectNextProxy()
		{
			while (true)
			{
				if (_normalQueue.Count < ProxyValidatorConfig.UseThresold)
				{
					Thread.Sleep(5000);
					continue;
				}
				if (!_normalQueue.TryDequeue(out var proxy))
				{
					Thread.Sleep(1500);
					continue;
				}
				return proxy;
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

		private void LoadProxies(int maxCount = 0)
		{
			var uris = _proxyUriLoader.Load(maxCount);
			foreach (var uri in uris)
			{
				_verifyingQueue.Enqueue(new WebProxy(uri));
			}
		}

		private void VerifyProxies()
		{
			while (true)
			{
				if (_normalQueue.Count >= ProxyValidatorConfig.VerifyPauseThresold)
				{
					Thread.Sleep(10000);
					continue;
				}
				if (_verifyingQueue.IsEmpty)
				{
					LoadProxies(10000);
				}
				var tasks = new List<Task>(ProxyValidatorConfig.VerifyTaskDegree);
				for (int i = 0; i < ProxyValidatorConfig.VerifyTaskDegree; i++)
				{
					if (!_verifyingQueue.TryDequeue(out var proxy))
					{
						break;
					}
					tasks.Add(VerifyProxyAsync(proxy));
				}
				if (tasks.Count > 0)
				{
					try
					{
						Task.WaitAll(tasks.ToArray(), ProxyValidatorConfig.VerifyTaskTimeout);
					}
					catch
					{ }
				}
			}
		}

		private async Task VerifyProxyAsync(WebProxy proxy)
		{
			bool isPassed = await _proxyValidator.VerifyProxyAsync(proxy, TargetUri);
			if (isPassed)
			{
				_normalQueue.Enqueue(proxy);
			}
		}
	}
}