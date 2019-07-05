using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SpiderX.BusinessBase
{
	public abstract class BllBase
	{
		private static ILogger _logger;

		protected static ILogger Logger => _logger;

		public BllBase(ILogger logger, string[] runSetting, int version)
		{
			Interlocked.CompareExchange(ref _logger, logger, null);
			RunSettings = runSetting;
			Version = version;
		}

		private string _className;

		public string ClassName
		{
			get
			{
				if (_className == null)
				{
					_className = GetType().Name;
				}
				return _className;
			}
		}

		public string[] RunSettings { get; }

		public int Version { get; }

		public virtual Task RunAsync()
		{
			return Task.CompletedTask;
		}

		protected static void ShowConsoleMsg(string msg)
		{
#if DEBUG
			Logger?.LogInformation(DateTime.Now.ToString("[MM/dd-hh:mm:ss] ") + msg);
#endif
		}
	}
}