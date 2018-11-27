using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxySelector : IProxySelector<SpiderProxy>
	{
		public SpiderProxySelector(Expression<Func<SpiderProxyEntity, bool>> loadCondition)
		{
			LoadCondition = loadCondition;
		}

		private readonly ConcurrentQueue<SpiderProxy> _queue = new ConcurrentQueue<SpiderProxy>();

		public bool HasNextProxy => !_queue.IsEmpty;

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
			foreach (var e in proxyEntities)
			{
				_queue.Enqueue(e.Value);
			}
			return _queue.Count;
		}

		public SpiderProxy SingleProxy()
		{
			throw new NotImplementedException();
		}
	}
}