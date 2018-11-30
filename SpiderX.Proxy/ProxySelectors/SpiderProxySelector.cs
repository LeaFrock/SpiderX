using System;
using System.Collections.Concurrent;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxySelector : IProxySelector<SpiderProxy>
	{
		public SpiderProxySelector(Func<SpiderProxyEntity, bool> entityLoadCondition)
		{
			EntityLoadCondition = entityLoadCondition;
		}

		private readonly ConcurrentQueue<SpiderProxy> _availableQueue = new ConcurrentQueue<SpiderProxy>();

		private readonly ConcurrentQueue<SpiderProxy> _verifyingQueue = new ConcurrentQueue<SpiderProxy>();

		private readonly ConcurrentQueue<SpiderProxy> _eliminatedQueue = new ConcurrentQueue<SpiderProxy>();

		public bool HasNextProxy => !_availableQueue.IsEmpty;

		public Func<SpiderProxyEntity, bool> EntityLoadCondition { get; }

		public bool CheckLoad(SpiderProxyEntity entity) => EntityLoadCondition == null || EntityLoadCondition(entity);

		public int LoadFrom<TContext>(ProxyAgent<TContext> agent) where TContext : ProxyDbContext
		{
			var proxyEntities = agent.SelectProxyEntities(CheckLoad, 7, 10000);
			foreach (var e in proxyEntities)
			{
				_availableQueue.Enqueue(e.Value);
			}
			return _availableQueue.Count;
		}

		public SpiderProxy SingleProxy()
		{
			if (_availableQueue.TryDequeue(out SpiderProxy result))
			{
				_availableQueue.Enqueue(result);
				return result;
			}
			return null;
		}
	}
}