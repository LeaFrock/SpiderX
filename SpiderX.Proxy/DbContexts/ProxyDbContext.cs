using Microsoft.EntityFrameworkCore;
using SpiderX.DataClient;

namespace SpiderX.Proxy
{
	public abstract class ProxyDbContext : DbContext, IProxyDbContext
	{
		public ProxyDbContext(DbConfig dbConfig) : base()
		{
			Config = dbConfig;
		}

		public DbConfig Config { get; }

		public DbSet<SpiderProxyEntity> ProxyEntity { get; set; }
	}
}