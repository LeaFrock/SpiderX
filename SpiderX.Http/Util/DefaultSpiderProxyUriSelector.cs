using System;
using System.Collections.Concurrent;

namespace SpiderX.Http.Util
{
	public sealed partial class DefaultSpiderProxyUriSelector
	{
		private readonly ConcurrentQueue<Uri> _internalQueue = new ConcurrentQueue<Uri>();

		private readonly ConcurrentDictionary<Uri, DateTime> _latestUseTimeRecords = new ConcurrentDictionary<Uri, DateTime>();

		private int _initTimes;

		public TimeSpan ProxyUriInterval { get; set; } = TimeSpan.Zero;
	}
}