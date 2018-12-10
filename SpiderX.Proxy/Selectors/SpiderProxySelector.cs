using System;
using System.Collections.Concurrent;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxySelector : IProxySelector<SpiderProxy>
	{
		public SpiderProxySelector(SpiderProxyValidator validator, Func<SpiderProxyEntity, bool> entityLoadCondition)
		{
			Validator = validator;
			EntityLoadCondition = entityLoadCondition;
		}

		private readonly ConcurrentQueue<SpiderProxy> _availableQueue = new ConcurrentQueue<SpiderProxy>();

		private readonly ConcurrentQueue<SpiderProxy> _verifyingQueue = new ConcurrentQueue<SpiderProxy>();

		private readonly ConcurrentQueue<SpiderProxy> _eliminatedQueue = new ConcurrentQueue<SpiderProxy>();

		public Func<SpiderProxyEntity, bool> EntityLoadCondition { get; }

		#region Interface Part

		public bool HasNextProxy => !_availableQueue.IsEmpty;

		public SpiderProxyValidator Validator { get; }

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

		#endregion Interface Part
	}
}