using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace SpiderX.Launcher
{
	public sealed class AppSettingManager
	{
		private static AppSettingManager _instance;

		public static AppSettingManager Instance
		{
			get
			{
				if (_instance == null)
				{
					if (Interlocked.CompareExchange(ref _instance, new AppSettingManager(), null) == null)
					{
						_instance.Initialize();
					}
				}
				return _instance;
			}
		}

		private static string CorrectCaseName(string name)
		{
			string trimName = name.Trim();
			if (!trimName.EndsWith("Bll", StringComparison.CurrentCultureIgnoreCase))
			{
				trimName += "Bll";
			}
			return trimName;
		}

		private static string CorrectBusinessDllName(string name)
		{
			string trimName = name.Trim();
			if (trimName.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase))
			{
				return trimName;
			}
			return trimName + ".dll";
		}

		private AppSettingManager()
		{
		}

		private List<DbConfig> _dbConfigs;

		public IReadOnlyList<DbConfig> DbConfigs => _dbConfigs;

		public string CaseName { get; private set; }

		public string[] CaseParams { get; private set; }

		public string BusinessDllName { get; private set; }

		public bool AutoClose { get; private set; } = true;

		public DbConfig FindConfig(string name, bool isTest)
		{
			return _dbConfigs.Find(p => p.IsTest == isTest && p.Name == name);
		}

		private void Initialize()
		{
			string filePath = Path.Combine(Directory.GetCurrentDirectory(), "AppSettings");
			var conf = new ConfigurationBuilder()
				.SetBasePath(filePath)
				.AddJsonFile("appsettings.json", true, true)
				.Build();
			//Load .dll Name
			string dllName = conf.GetSection(nameof(BusinessDllName)).Value;
			if (string.IsNullOrWhiteSpace(dllName))
			{
				throw new ArgumentNullException("Invalid BusinessDllName.");
			}
			BusinessDllName = CorrectBusinessDllName(dllName);
			//Load Case Name&Params
			string bllName = conf.GetSection(nameof(CaseName)).Value;
			if (string.IsNullOrWhiteSpace(bllName))
			{
				throw new ArgumentNullException("CaseName is Null or WhiteSpace");
			}
			CaseName = CorrectCaseName(bllName);
			var bllParams = conf.GetSection(nameof(CaseParams)).GetChildren();
			CaseParams = bllParams.Select(p => p.Value).ToArray();
			//Load DbConfigs
			var dbConfigs = conf.GetSection(nameof(DbConfigs)).GetChildren();
			_dbConfigs = new List<DbConfig>();
			foreach (var dbSection in dbConfigs)
			{
				DbConfig item = DbConfig.CreateInstance(dbSection);
				if (item != null)
				{
					_dbConfigs.Add(item);
				}
			}
			if (_dbConfigs.Count < 1)
			{
				throw new ArgumentException("No DbConfigs Valid.");
			}
			//Load Other
			var autoCloseStr = conf.GetSection(nameof(AutoClose)).Value;
			if (bool.TryParse(autoCloseStr, out bool autoClose))
			{
				AutoClose = autoClose;
			}
		}
	}
}