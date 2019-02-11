using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SpiderX.Proxy;
using SpiderX.Tools;

namespace SpiderX.Http.Util
{
	public sealed class DefaultSpiderProxySelector : SpiderProxySelectorBase
	{
		public DefaultSpiderProxySelector() : this(100)
		{
		}

		public DefaultSpiderProxySelector(int degreeOfParallelism)
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

		public int DegreeOfParallelism { get; }

		private void StartVerifying()
		{
			if (DegreeOfParallelism > 1)
			{
				Parallel.For(0, DegreeOfParallelism, VerifyContinuously);
			}
			else
			{
				VerifyContinuously(0);
			}
		}

		private void VerifyContinuously(int id)
		{
			while (true)
			{
				VerifySingle();
			}
		}

		private void VerifySingle()
		{
			if (!_verifyingQueue.TryDequeue(out SpiderProxy proxy))
			{
				Thread.Sleep(CommonTool.RandomEvent.Next(3000, 7000));//Prevent high CPU occupancy.
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

		#region Override Part

		public override bool HasNextProxy => !_availableQueue.IsEmpty;

		public override int StatusCode => _initTimes;

		public override void Init(IProxyValidator Validator, IEnumerable<SpiderProxy> proxies)
		{
			if (Interlocked.CompareExchange(ref _initTimes, 1, 0) != 0)
			{
				return;
			}
			InsertFreshProxies(proxies);
			Task.Factory.StartNew(StartVerifying, TaskCreationOptions.LongRunning);
		}

		public override void InsertFreshProxies(IEnumerable<SpiderProxy> proxies)
		{
			foreach (var proxy in proxies)
			{
				_verifyingQueue.Enqueue(proxy);
			}
		}

		public override SpiderProxy SingleProxy()
		{
			if (_availableQueue.TryDequeue(out SpiderProxy result))
			{
				_availableQueue.Enqueue(result);
				return result;
			}
			return null;
		}

		#endregion Override Part
	}
}