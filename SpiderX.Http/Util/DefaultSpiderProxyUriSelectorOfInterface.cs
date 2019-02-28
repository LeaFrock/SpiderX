using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.Http.Util
{
	public sealed partial class DefaultSpiderProxyUriSelector : SpiderProxyUriSelectorBase
	{
		public override bool HasNextProxy => !_availableQueue.IsEmpty;

		public override int StatusCode => _initTimes;

		public override void Init(IEnumerable<Uri> proxies)
		{
			if (Interlocked.CompareExchange(ref _initTimes, 1, 0) != 0)
			{
				return;
			}
			InsertFreshProxies(proxies);
			Task.Factory.StartNew(StartVerifying, TaskCreationOptions.LongRunning);
		}

		public override void InsertFreshProxies(IEnumerable<Uri> proxies)
		{
			foreach (var proxy in proxies)
			{
				_verifyingQueue.Enqueue(proxy);
			}
		}

		public override Uri SingleProxyUri()
		{
			if (ProxyUriInterval <= TimeSpan.Zero)
			{
				while (true)
				{
					if (_availableQueue.TryDequeue(out Uri uri))
					{
						_availableQueue.Enqueue(uri);
						return uri;
					}
					Thread.Sleep(CommonTool.RandomEvent.Next(3000, 7000));
				}
			}
			else
			{
				while (true)
				{
					if (_availableQueue.TryDequeue(out Uri uri))
					{
						_availableQueue.Enqueue(uri);
						DateTime dt = _latestUseTimeRecords.GetOrAdd(uri, DateTime.MinValue);
						DateTime now = DateTime.Now;
						if (now > dt.Add(ProxyUriInterval))
						{
							_latestUseTimeRecords.TryUpdate(uri, now, now);
							return uri;
						}
					}
					Thread.Sleep(CommonTool.RandomEvent.Next(3000, 7000));
				}
			}
		}
	}
}