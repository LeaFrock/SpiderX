using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SpiderX.BusinessBase
{
	public abstract class BllBase
	{
		public BllBase()
		{ }

		public BllBase(ILogger logger, string[] runSetting, int version)
		{
			Logger = logger;
			RunSetting = runSetting;
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

		public IReadOnlyList<string> RunSetting { get; }

		public int Version { get; }

		public abstract Task RunAsync(params string[] args);

		public virtual Task RunAsync()
		{
			return Task.CompletedTask;
		}

		protected static void ShowConsoleMsg(string msg)
		{
#if DEBUG
			Console.WriteLine(DateTime.Now.ToString("[MM/dd-hh:mm:ss] ") + msg);
#else
			Console.WriteLine(msg);
#endif
		}
	}
}