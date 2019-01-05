using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;

namespace SpiderX.Http
{
	public sealed class SpiderWebClientPool
	{
		public SpiderWebClientPool() : this(500)
		{
		}

		public SpiderWebClientPool(int capacity)
		{
			if (capacity < 2)
			{
				throw new ArgumentOutOfRangeException("capacity is less than 2.");
			}
			for (int i = 0; i < capacity; i++)
			{
				_clientQueue.Enqueue(new SpiderWebClient());
			}
		}

		private readonly ConcurrentQueue<SpiderWebClient> _clientQueue = new ConcurrentQueue<SpiderWebClient>();

		public SpiderWebClient DischargeOnce(IWebProxy webProxy)
		{
			if (_clientQueue.TryDequeue(out SpiderWebClient webClient))
			{
				webClient.SetProxy(webProxy);
			}
			return webClient;
		}

		public SpiderWebClient Distribute(IWebProxy webProxy)
		{
			SpiderWebClient webClient = null;
			SpinWait.SpinUntil(() => _clientQueue.TryDequeue(out webClient), Timeout.Infinite);
			if (webClient == null)
			{
				throw new InvalidOperationException();
			}
			webClient.SetProxy(webProxy);
			return webClient;
		}

		public void Recycle(SpiderWebClient webClient)
		{
			_clientQueue.Enqueue(webClient);
		}
	}
}