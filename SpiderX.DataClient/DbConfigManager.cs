using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SpiderX.DataClient
{
	public sealed class DbConfigManager
	{
		public static DbConfigManager Default { get; private set; }

		public DbConfigManager(IConfiguration configuration)
		{
			Initialize(configuration);
		}

		private List<DbConfig> _dbConfigs;

		public IReadOnlyList<DbConfig> DbConfigs => _dbConfigs;

		private string _proxyDbConfigName;

		public DbConfig GetConfig(string name, bool? isTest = null)
		{
			if (isTest.HasValue)
			{
				return _dbConfigs.Find(p => p.IsTest == isTest && p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
			}
			return _dbConfigs.Find(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		public DbConfig GetDefaultConfig() => _dbConfigs[0];

		public DbConfig GetProxyConfig()
		{
			if (_proxyDbConfigName is null)
			{
				return GetDefaultConfig();
			}
			var c = _dbConfigs.Find(p => p.Name.Equals(_proxyDbConfigName, StringComparison.OrdinalIgnoreCase)) ?? GetDefaultConfig();
			return c;
		}

		private void Initialize(IConfiguration root)
		{
			var dbSections = root.GetSection(nameof(DbConfigs)).GetChildren();
			var configs = new List<DbConfig>();
			foreach (var dbSection in dbSections)
			{
				DbConfig item = DbConfig.FromConfiguration(dbSection);
				if (item != null)
				{
					configs.Add(item);
				}
			}
			if (configs.Count < 1)
			{
				throw new DbConfigNotFoundException(nameof(Initialize));
			}
			configs.TrimExcess();
			_dbConfigs = configs;
			_proxyDbConfigName = root.GetValue<string>("ProxyDbConfigName");
		}

		public static void SetDefault(IServiceProvider provider)
		{
			Default = provider.GetService<DbConfigManager>();
		}
	}
}