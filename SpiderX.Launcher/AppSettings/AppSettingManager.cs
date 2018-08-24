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

		private AppSettingManager()
		{
		}

		private List<DbConfig> _dbConfigs;

		public IReadOnlyList<DbConfig> DbConfigs => _dbConfigs;

		public string BllName { get; private set; }

		public string[] BllParams { get; private set; }

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
			var dbConfigs = conf.GetSection("DbConfigs").GetChildren();
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
			string bllName = conf.GetSection("BllName").Value;
			if (string.IsNullOrWhiteSpace(bllName))
			{
				throw new ArgumentNullException("BllName is Null or WhiteSpace");
			}
			BllName = CorrectName(bllName);
			var bllParams = conf.GetSection("BllParams").GetChildren();
			BllParams = bllParams.Select(p => p.Value).ToArray();
		}

		private static string CorrectName(string name)
		{
			string trimName = name.Trim();
			if (!trimName.EndsWith("Bll", StringComparison.CurrentCultureIgnoreCase))
			{
				trimName += "Bll";
			}
			return trimName;
		}
	}
}