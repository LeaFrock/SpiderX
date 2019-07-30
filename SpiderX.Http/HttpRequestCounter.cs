using System;
using System.Collections.Concurrent;
using System.Text;
using System.Timers;
using Microsoft.Extensions.Logging;

namespace SpiderX.Http
{
	public sealed class HttpRequestCounter
	{
		private readonly static object _timerSyncRoot = new object();

		private readonly static Lazy<Timer> _timerLazy = new Lazy<Timer>(CreateTimer, true);

		private readonly static StringBuilder _counterInfoBuilder = new StringBuilder(10);
		private readonly static ConcurrentDictionary<string, HttpRequestCounter> _counterCache = new ConcurrentDictionary<string, HttpRequestCounter>();

		internal static Timer Timer => _timerLazy.Value;

		public static ILogger Logger { get; set; }

		private HttpRequestCounter()
		{
		}

		public string Name { get; private set; }

		public long Send { get; private set; }

		public long Pass { get; private set; }

		public long Success { get; private set; }

		public void Reset()
		{
			Send = Pass = Success = 0;
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

		public static void Clear()
		{
			if (_timerLazy.IsValueCreated)
			{
				Timer?.Dispose();
			}
		}

		public static bool TryRegisterNew(string name)
		{
			var c = new HttpRequestCounter() { Name = name };
			bool success = _counterCache.TryAdd(name, c);
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

		private static Timer CreateTimer()
		{
			var timer = new Timer()
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
				_counterInfoBuilder.Append(counter.Success);
				string counterInfo = _counterInfoBuilder.ToString();
				_counterInfoBuilder.Clear();

				Logger.LogInformation(counterInfo);
			}
		}
	}
}