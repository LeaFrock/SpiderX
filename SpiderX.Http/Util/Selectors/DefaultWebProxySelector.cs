using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SpiderX.Proxy;

namespace SpiderX.Http.Util
{
	public sealed class DefaultWebProxySelector : IWebProxySelector
	{
		public DefaultWebProxySelector(Uri uri, IProxyUriLoader uriLoader, Func<IWebProxy, HttpClient> clientFactory, Predicate<string> rspValidator = null)
		{
			TargetUri = uri ?? throw new ArgumentNullException(nameof(TargetUri));
			_proxyUriLoader = uriLoader ?? throw new ArgumentNullException(nameof(_proxyUriLoader));
			_clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(_clientFactory));
			_responseValidator = rspValidator;
		}

		private readonly Predicate<string> _responseValidator;
		private readonly Func<IWebProxy, HttpClient> _clientFactory;
		private readonly IProxyUriLoader _proxyUriLoader;

		private readonly ConcurrentQueue<WebProxy> _verifyingQueue = new ConcurrentQueue<WebProxy>();
		private readonly ConcurrentQueue<WebProxy> _normalQueue = new ConcurrentQueue<WebProxy>();
		private readonly ConcurrentQueue<WebProxy> _advancedQueue = new ConcurrentQueue<WebProxy>();

		private readonly ConcurrentDictionary<string, byte> _failTimesRecordOfNormalProxies = new ConcurrentDictionary<string, byte>();
		private readonly ConcurrentDictionary<string, byte> _failTimesRecordOfAdvancedProxies = new ConcurrentDictionary<string, byte>();

		private int _initCode;

		public bool IsInited => _initCode != 0;

		public int UseThresold { get; set; } = 100;

		public int VerifyPauseThresold { get; set; } = 300;

		public int AdvancedProxiesCacheCapacity { get; set; } = 100;

		public int VerifyTaskDegree { get; set; } = 120;

		public TimeSpan VerifyTaskTimeout { get; set; } = TimeSpan.FromSeconds(20);

		public TimeSpan VerifyTimeout { get; set; } = TimeSpan.FromSeconds(3);

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
				if (_normalQueue.Count < UseThresold)
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
				if (_normalQueue.Count >= VerifyPauseThresold)
				{
					Thread.Sleep(10000);
					continue;
				}
				if (_verifyingQueue.IsEmpty)
				{
					LoadProxies(10000);
				}
				var tasks = new List<Task>(VerifyTaskDegree);
				for (int i = 0; i < VerifyTaskDegree; i++)
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
						Task.WaitAll(tasks.ToArray(), VerifyTaskTimeout);
					}
					catch
					{ }
				}
			}
		}

		private async Task VerifyProxyAsync(WebProxy proxy)
		{
			using (HttpClient client = _clientFactory.Invoke(proxy))
			{
				string rspText = await client.GetStringAsync(TargetUri);
				if (string.IsNullOrWhiteSpace(rspText))
				{
					return;
				}
				rspText = rspText.Trim();
				if (_responseValidator?.Invoke(rspText) != false)
				{
					_normalQueue.Enqueue(proxy);
				}
			}
		}
	}
}