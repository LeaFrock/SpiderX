using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using SpiderX.DataClient;

namespace SpiderX.Launcher
{
	public static class GlobalSetting
	{
		public static string DefaultNamespace { get; private set; }

		public static IReadOnlyList<CaseOption> CaseSettings { get; private set; }

		public static bool RunCasesConcurrently { get; private set; }

		public static bool AutoClose { get; private set; } = true;

		internal static void Initialize(IConfigurationRoot conf)
		{
			DefaultNamespace = conf.GetSection(nameof(DefaultNamespace)).Value;
			string autoCloseStr = conf.GetSection(nameof(AutoClose)).Value;
			if (bool.TryParse(autoCloseStr, out bool autoClose))
			{
				AutoClose = autoClose;
			}
			//Init DbClient
			DbClient.Initialize(conf);
		}
	}
}