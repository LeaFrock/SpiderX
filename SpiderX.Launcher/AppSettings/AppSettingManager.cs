using System;
using System.Collections.Generic;
using System.IO;
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

		private List<DbConfig> _dbConfigs = new List<DbConfig>();

		public IReadOnlyList<DbConfig> DbConfigs => _dbConfigs;

		private void Initialize()
		{
			string filePath = Path.Combine(Directory.GetCurrentDirectory(), "AppSettings");
			var conf = new ConfigurationBuilder()
				.SetBasePath(filePath)
				.AddJsonFile("appsettings.json", true, true)
				.Build();
			var dbConfigs = conf.GetSection("DbConfigs").GetChildren();
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
		}
	}
}