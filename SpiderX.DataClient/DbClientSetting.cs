using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace SpiderX.DataClient
{
	internal class DbClientSetting
	{
		public DbClientSetting(List<DbConfig> configs)
		{
			_dbConfigs = configs;
		}

		private readonly List<DbConfig> _dbConfigs;

		public IReadOnlyList<DbConfig> DbConfigs => _dbConfigs;

		public DbConfig FindConfig(string name, bool isTest)
		{
			return _dbConfigs.Find(p => p.IsTest == isTest && p.Name == name);
		}

		internal static DbClientSetting CreateInstance(IConfigurationRoot root)
		{
			var dbSections = root.GetSection(nameof(DbConfigs)).GetChildren();
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
				throw new DbConfigNotFoundException(nameof(CreateInstance));
			}
			return new DbClientSetting(result);
		}
	}
}