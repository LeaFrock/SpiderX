using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using SpiderX.Tools;

namespace SpiderX.Http.Util
{
	public sealed partial class DefaultSpiderProxyUriSelector
	{
		public DefaultSpiderProxyUriSelector() : this(100)
		{
		}

		public DefaultSpiderProxyUriSelector(int degreeOfParallelism)
		{
			if (degreeOfParallelism < 1)
			{
				throw new ArgumentOutOfRangeException("degreeOfParallelism is less than 1.");
			}
			DegreeOfParallelism = degreeOfParallelism;
		}

		private readonly ConcurrentQueue<Uri> _availableQueue = new ConcurrentQueue<Uri>();

		private readonly ConcurrentQueue<Uri> _verifyingQueue = new ConcurrentQueue<Uri>();

		private readonly ConcurrentQueue<Uri> _eliminatedQueue = new ConcurrentQueue<Uri>();

		private readonly ConcurrentDictionary<Uri, DateTime> _latestUseTimeRecords = new ConcurrentDictionary<Uri, DateTime>();

		private int _initTimes;

		public int DegreeOfParallelism { get; }

		public TimeSpan ProxyUriInterval { get; set; } = TimeSpan.Zero;

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
			if (!_verifyingQueue.TryDequeue(out Uri proxy))
			{
				Thread.Sleep(CommonTool.RandomEvent.Next(3000, 7000));//Prevent high CPU occupancy.
				return;
			}
			//if (Validator.CheckPass(proxy))
			//{
			//	_availableQueue.Enqueue(proxy);
			//}
			//else
			//{
			//	_eliminatedQueue.Enqueue(proxy);
			//}
		}
	}
}