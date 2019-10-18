using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SpiderX.BusinessBase
{
	public abstract class BllBase
	{
		private static ILogger _logger;

		protected static ILogger Logger => _logger;

		public BllBase(ILogger logger, string[] runSetting, string dbConfigName, int version)
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

		public string DbConfigName { get; }

		public int Version { get; }

		public abstract Task RunAsync();

		protected static void ShowLogInfo(string msg)
		{
#if DEBUG
			Logger?.LogInformation(DateTime.Now.ToString("[MM/dd-hh:mm:ss] ") + msg);
#endif
		}

		protected static void ShowLogError(string msg)
		{
			Logger?.LogError(DateTime.Now.ToString("[MM/dd-hh:mm:ss] ") + msg);
		}

		protected static void ShowLogException(Exception ex)
		{
			if (Logger == null)
			{
				return;
			}
			List<string> args = new List<string>(2) { ex.Message };
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
				args.Add(ex.Message);
			}
			Logger.LogError(ex, DateTime.Now.ToString("[MM/dd-hh:mm:ss] "), args);
		}
	}
}