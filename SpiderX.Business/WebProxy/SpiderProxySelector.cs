using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxySelector : IProxySelector<SpiderProxy>
	{
		public SpiderProxySelector() : this(100)
		{
		}

		public SpiderProxySelector(int degreeOfParallelism)
		{
			if (degreeOfParallelism < 1)
			{
				throw new ArgumentOutOfRangeException("degreeOfParallelism is less than 1.");
			}
			DegreeOfParallelism = degreeOfParallelism;
		}

		private readonly ConcurrentQueue<SpiderProxy> _availableQueue = new ConcurrentQueue<SpiderProxy>();

		private readonly ConcurrentQueue<SpiderProxy> _verifyingQueue = new ConcurrentQueue<SpiderProxy>();

		private readonly ConcurrentQueue<SpiderProxy> _eliminatedQueue = new ConcurrentQueue<SpiderProxy>();

		private int _initTimes;

		public int StatusCode => _initTimes;

		public int DegreeOfParallelism { get; }

		private void Verify()
		{
			if (DegreeOfParallelism > 1)
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
			else
			{
				while (true)
				{
					VerifySingle();
				}
			}
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

		public IProxyLoader Loader { get; set; }

		public IProxyValidator Validator { get; set; }

		public void Init(IProxyValidator Validator, IEnumerable<SpiderProxy> proxies)
		{
			if (Interlocked.CompareExchange(ref _initTimes, 1, 0) != 0)
			{
				return;
			}
			InsertFreshProxies(proxies);
			Task.Factory.StartNew(Verify);
		}

		public void InsertFreshProxies(IEnumerable<SpiderProxy> proxies)
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