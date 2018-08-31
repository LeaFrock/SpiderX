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

		public string BusinessModuleName { get; private set; }

		public string BusinessModulePath { get; private set; }

		private BusinessModuleCopyModeEnum _businessModuleCopyMode;

		public BusinessModuleCopyModeEnum BusinessModuleCopyMode => _businessModuleCopyMode;

		public bool AutoClose { get; private set; } = true;

		public DbConfig FindConfig(string name, bool isTest)
		{
			return _dbConfigs.Find(p => p.IsTest == isTest && p.Name == name);
		}

		public void CopyModuleTo(string destDirectoryPath)
		{
			if (string.IsNullOrWhiteSpace(BusinessModulePath))
			{
				throw new ArgumentException("Modules Load Fail: Invalid BusinessModulePath.");
			}
			if (!Directory.Exists(BusinessModulePath))
			{
				throw new DirectoryNotFoundException("Modules Load Fail: " + BusinessModulePath);
			}
			string sourceFile = Path.Combine(BusinessModulePath, BusinessModuleName);
			if (!File.Exists(sourceFile))
			{
				throw new FileNotFoundException("Modules Load Fail: " + sourceFile);
			}
			string destFileName = Path.Combine(destDirectoryPath, BusinessModuleName);
			File.Copy(sourceFile, destFileName, true);
		}

		private void Initialize()
		{
			string filePath = Path.Combine(Directory.GetCurrentDirectory(), "AppSettings");
			var conf = new ConfigurationBuilder()
				.SetBasePath(filePath)
				.AddJsonFile("appsettings.json", true, true)
				.Build();
			//Load .dll Name
			string dllName = conf.GetSection(nameof(BusinessModuleName)).Value;
			if (string.IsNullOrWhiteSpace(dllName))
			{
				throw new ArgumentNullException("Invalid BusinessModuleName.");
			}
			BusinessModuleName = CorrectBusinessDllName(dllName);
			BusinessModulePath = conf.GetSection(nameof(BusinessModulePath)).Value;
			string copyMode = conf.GetSection(nameof(BusinessModuleCopyMode)).Value;
			Enum.TryParse(copyMode, true, out _businessModuleCopyMode);
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

	public enum BusinessModuleCopyModeEnum : sbyte
	{
		CopyOnce = 0,//Copy only when the module doesn't exist
		AlwaysCopy = 1,//Copy or Overwrite the module
	}
}