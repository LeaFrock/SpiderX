using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxySelector : IProxySelector<SpiderProxy>
	{
		public SpiderProxySelector(int degreeOfParallelism, Predicate<SpiderProxyEntity> entityLoadCondition)
		{
			DegreeOfParallelism = degreeOfParallelism;
			EntityLoadCondition = entityLoadCondition;
		}

		private readonly ConcurrentQueue<SpiderProxy> _availableQueue = new ConcurrentQueue<SpiderProxy>();

		private readonly ConcurrentQueue<SpiderProxy> _verifyingQueue = new ConcurrentQueue<SpiderProxy>();

		private readonly ConcurrentQueue<SpiderProxy> _eliminatedQueue = new ConcurrentQueue<SpiderProxy>();

		private int _initTimes;

		public int StatusCode => _initTimes;

		public int DegreeOfParallelism { get; } = 100;

		public Predicate<SpiderProxyEntity> EntityLoadCondition { get; }

		private void Verify()
		{
			Parallel.For(0, DegreeOfParallelism,
				i =>
				{
					while (true)
					{
						VerifySingle();
					}
				});
		}

		private void VerifySingle()
		{
			if (!_verifyingQueue.TryDequeue(out SpiderProxy proxy))
			{
				return;
			}
			if (Validator.CheckPass(proxy))
			{
				_availableQueue.Enqueue(proxy);
			}
			else
			{
				_eliminatedQueue.Enqueue(proxy);
			}
		}

		#region Interface Part

		public bool HasNextProxy => !_availableQueue.IsEmpty;

		public SpiderProxyValidator Validator { get; set; }

		public bool CheckLoad(SpiderProxyEntity entity) => EntityLoadCondition?.Invoke(entity) ?? true;

		public void Init(SpiderProxyValidator Validator, IEnumerable<SpiderProxy> proxies)
		{
			if (Interlocked.CompareExchange(ref _initTimes, 1, 0) != 0)
			{
				return;
			}
			InsertProxies(proxies);
			Task.Factory.StartNew(Verify);
		}

		public void InsertProxies(IEnumerable<SpiderProxy> proxies)
		{
			foreach (var proxy in proxies)
			{
				_verifyingQueue.Enqueue(proxy);
			}
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