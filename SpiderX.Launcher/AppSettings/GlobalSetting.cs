using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using SpiderX.DataClient;

namespace SpiderX.Launcher
{
	public static class GlobalSetting
	{
		public static IReadOnlyList<CaseSetting> CaseSettings { get; private set; }

		public static bool RunCasesConcurrently { get; private set; }

		public static bool AutoClose { get; private set; } = true;

		internal static void Initialize(IReadOnlyList<CaseSetting> cases)
		{
			string filePath = Path.Combine(Directory.GetCurrentDirectory(), "AppSettings");
			var conf = new ConfigurationBuilder()
				.SetBasePath(filePath)
				.AddJsonFile("appsettings.json", true, true)
				.Build();
			//Load CaseSettings
			CaseSettings = cases ?? LoadCaseSettings(conf);
			//Load DbConfigs
			var dbConfigs = LoadDbConfigs(conf);
			DbClientSetting.Instance.InitializeConfigs(dbConfigs);
			//Load Other
			if (CaseSettings.Count > 1)
			{
				string runCasesConcurrentlyStr = conf.GetSection(nameof(RunCasesConcurrently)).Value;
				if (bool.TryParse(runCasesConcurrentlyStr, out bool runCasesConcurrently))
				{
					RunCasesConcurrently = runCasesConcurrently;
				}
			}
			string autoCloseStr = conf.GetSection(nameof(AutoClose)).Value;
			if (bool.TryParse(autoCloseStr, out bool autoClose))
			{
				AutoClose = autoClose;
			}
		}

		private static List<CaseSetting> LoadCaseSettings(IConfigurationRoot root)
		{
			var dbSections = root.GetSection("CaseSettings").GetChildren();
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

		private static List<DbConfig> LoadDbConfigs(IConfigurationRoot root)
		{
			var dbSections = root.GetSection("DbConfigs").GetChildren();
			var result = new List<DbConfig>();
			foreach (var dbSection in dbSections)
			{
				DbConfig item = DbConfig.FromConfiguration(dbSection);
				if (item != null)
				{
					result.Add(item);
				}
			}
			if (result.Count < 1)
			{
				throw new ArgumentException("Invalid DbConfigs.");
			}
			return result;
		}
	}
}