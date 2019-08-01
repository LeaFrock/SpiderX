using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Timers;
using Microsoft.Extensions.Logging;

namespace SpiderX.Http
{
	public sealed class HttpRequestCounter
	{
		private readonly static object _timerSyncRoot = new object();

		private readonly static Lazy<System.Timers.Timer> _timerLazy = new Lazy<System.Timers.Timer>(CreateTimer, true);

		private readonly static StringBuilder _counterInfoBuilder = new StringBuilder(10);
		private readonly static ConcurrentDictionary<string, HttpRequestCounter> _counterCache = new ConcurrentDictionary<string, HttpRequestCounter>();

		internal static System.Timers.Timer Timer => _timerLazy.Value;

		public static ILogger Logger { get; set; }

		private HttpRequestCounter()
		{
		}

		private long _send;
		private long _pass;
		private long _succeed;

		public string Name { get; private set; }

		public long Send => _send;

		public long Pass => _pass;

		public long Succeed => _succeed;

		public void Reset()
		{
			Interlocked.Exchange(ref _send, 0);
			Interlocked.Exchange(ref _pass, 0);
			Interlocked.Exchange(ref _succeed, 0);
		}

		public void Disable()
		{
			if (_counterCache.TryRemove(Name, out var _))
			{
				lock (_timerSyncRoot)
				{
					if (_counterCache.IsEmpty)
					{
						Timer.Stop();
					}
				}
			}
		}

		public void OnSend()
		{
			Interlocked.Increment(ref _send);
		}

		public void OnPass()
		{
			Interlocked.Increment(ref _pass);
		}

		public void OnSucceed()
		{
			Interlocked.Increment(ref _succeed);
		}

		public static void DisposeStaticTimer()
		{
			if (_timerLazy.IsValueCreated)
			{
				Timer?.Stop();
				Timer?.Dispose();
			}
		}

		public static bool TryRegisterNew(string name, out HttpRequestCounter counter)
		{
			counter = new HttpRequestCounter() { Name = name };
			bool success = _counterCache.TryAdd(name, counter);
			if (success)
			{
				lock (_timerSyncRoot)
				{
					if (!_timerLazy.IsValueCreated || !Timer.Enabled)
					{
						Timer.Start();
					}
				}
			}
			return success;
		}

		private static System.Timers.Timer CreateTimer()
		{
			var timer = new System.Timers.Timer()
			{
				AutoReset = true,
				Enabled = false,
				Interval = 60000d
			};
			timer.Elapsed += OnTimeCheck;
			return timer;
		}

		private static void OnTimeCheck(object sender, ElapsedEventArgs args)
		{
			if (Logger == null)
			{
				return;
			}
			string currentTime = args.SignalTime.ToString("[MM/dd-hh:mm:ss]");
			foreach (var counter in _counterCache.Values)
			{
				_counterInfoBuilder.Append(currentTime);
				_counterInfoBuilder.Append('[');
				_counterInfoBuilder.Append(counter.Name);
				_counterInfoBuilder.Append(']');
				_counterInfoBuilder.Append("Send/Pass/Success: ");
				_counterInfoBuilder.Append(counter.Send);
				_counterInfoBuilder.Append('/');
				_counterInfoBuilder.Append(counter.Pass);
				_counterInfoBuilder.Append('/');
				_counterInfoBuilder.Append(counter.Succeed);
				string counterInfo = _counterInfoBuilder.ToString();
				_counterInfoBuilder.Clear();

				Logger.LogInformation(counterInfo);
			}
		}
	}
}