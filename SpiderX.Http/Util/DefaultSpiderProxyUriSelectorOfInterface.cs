using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SpiderX.Proxy;

namespace SpiderX.Http.Util
{
	public sealed partial class DefaultSpiderProxyUriSelector : SpiderProxyUriSelectorBase
	{
		public override bool HasNextProxy => !_availableQueue.IsEmpty;

		public override int StatusCode => _initTimes;

		public override void Init(IProxyUriValidator Validator, IEnumerable<Uri> proxies)
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
			if (_availableQueue.TryDequeue(out Uri result))
			{
				_availableQueue.Enqueue(result);
				return result;
			}
			return null;
		}
	}
}