using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SpiderX.DataClient;

namespace SpiderX.Proxy
{
	public abstract class ProxyDbContext : DbContext
	{
		public ProxyDbContext(DbConfig dbConfig, string dbName) : base()
		{
			Config = dbConfig;
			DbName = dbName ?? "SpiderProxy";
		}

		public DbConfig Config { get; }

		public string DbName { get; }

		public DbSet<SpiderProxyUriEntity> ProxyUriEntities { get; set; }

		public static ProxyDbContext CreateInstance(string dbConfigName = null)
		{
			var c = dbConfigName is null
				? DbConfigManager.Default.GetProxyConfig()
				: DbConfigManager.Default.GetConfig(dbConfigName);
			if (c is null)
			{
				throw new DbConfigNotFoundException($"Config '{dbConfigName}' not found.");
			}
			return c.Type switch
			{
				DbEnum.SqlServer => new SqlServerProxyDbContext(c, null),
				DbEnum.MySql => new MySqlProxyDbContext(c, null),
				_ => throw new NotSupportedException($"Config '{dbConfigName}' not supported."),
			};
		}

		public abstract ICollection<SpiderProxyUriEntity> SelectProxyEntities(ISpiderProxyUriEntityOption entityOption, int recentDays, int count);

		public abstract int InsertProxyEntities(IEnumerable<SpiderProxyUriEntity> entities);

		public abstract int UpdateProxyEntity(int id, Action<SpiderProxyUriEntity> updateAction);

		public abstract int UpdateProxyEntities(IEnumerable<int> ids, Action<SpiderProxyUriEntity> updateAction);

		public abstract int DeleteProxyEntity(int id);

		public abstract int DeleteProxyEntity(string host, int port);
	}
}