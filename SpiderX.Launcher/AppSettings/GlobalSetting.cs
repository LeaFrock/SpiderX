using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using SpiderX.DataClient;

namespace SpiderX.Launcher
{
	public static class GlobalSetting
	{
		public static string DefaultNamespace { get; private set; }

		public static IReadOnlyList<CaseSetting> CaseSettings { get; private set; }

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

		internal static void LoadCaseSettings(IConfigurationRoot conf, IReadOnlyList<CaseSetting> settings = null)
		{
			if (settings == null || settings.Count < 1)
			{
				CaseSettings = LoadCaseSettings(conf);
			}
			else
			{
				CaseSettings = settings;
			}
			if (CaseSettings.Count > 1)
			{
				string runCasesConcurrentlyStr = conf.GetSection(nameof(RunCasesConcurrently)).Value;
				if (bool.TryParse(runCasesConcurrentlyStr, out bool runCasesConcurrently))
				{
					RunCasesConcurrently = runCasesConcurrently;
				}
			}
		}

		private static List<CaseSetting> LoadCaseSettings(IConfigurationRoot root)
		{
			var dbSections = root.GetSection(nameof(CaseSettings)).GetChildren();
			var result = new List<CaseSetting>();
			foreach (var dbSection in dbSections)
			{
				CaseSetting item = CaseSetting.FromConfiguration(dbSection);
				if (item != null)
				{
					result.Add(item);
				}
			}
			if (result.Count < 1)
			{
				throw new ArgumentException("Invalid CaseSettings.");
			}
			return result;
		}
	}
}