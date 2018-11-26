using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxySelector : IProxySelector<SpiderProxy>
	{
		public SpiderProxySelector(Expression<Func<SpiderProxyEntity, bool>> loadCondition)
		{
			LoadCondition = loadCondition;
		}

		private ConcurrentQueue<SpiderProxy> _queue;

		public bool HasNextProxy => _queue != null && !_queue.IsEmpty;

		public Expression<Func<SpiderProxyEntity, bool>> LoadCondition { get; }

		public int LoadFrom(ProxyAgent agent)
		{
			if (HasNextProxy)
			{
				return -1;
			}
			var proxyEntities = LoadCondition == null
				? agent.SelectProxyEntities(p => p.UpdateTime > DateTime.Now.AddDays(-10))
				: agent.SelectProxyEntities(LoadCondition);
			var tempQueue = new ConcurrentQueue<SpiderProxy>(proxyEntities.Select(p => p.Value));
			if (Interlocked.CompareExchange(ref _queue, tempQueue, null) != null)
			{
				return -2;
			}
			return _queue.Count;
		}

		public SpiderProxy SingleProxy()
		{
			throw new NotImplementedException();
		}
	}
}