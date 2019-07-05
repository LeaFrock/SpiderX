using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SpiderX.BusinessBase
{
	public abstract class BllBase
	{
		public BllBase(ILogger logger, string[] runSetting, int version)
		{
			Logger = logger;
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

		protected ILogger Logger { get; }

		public IReadOnlyList<string> RunSettings { get; }

		public int Version { get; }

		public virtual Task RunAsync()
		{
			return Task.CompletedTask;
		}

		protected void ShowConsoleMsg(string msg)
		{
#if DEBUG
			Logger.LogInformation(DateTime.Now.ToString("[MM/dd-hh:mm:ss] ") + msg);
#endif
		}
	}
}