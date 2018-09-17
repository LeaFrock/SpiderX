using Microsoft.EntityFrameworkCore;
using SpiderX.DataClient;

namespace SpiderX.Proxy
{
	public sealed class ProxyDbContext : DbContext
	{
		public ProxyDbContext(DbConfig dbConfig) : base()
		{
			Config = dbConfig;
		}

		public DbConfig Config { get; }

		public DbSet<SpiderProxyEntity> ProxyEntities { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseSqlServer(Config.ConnectionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<SpiderProxyEntity>()
				.HasKey(e => e.Id);
			modelBuilder.Entity<SpiderProxyEntity>()
				.Property(e => e.Id)
				.ValueGeneratedOnAdd();
			modelBuilder.Entity<SpiderProxyEntity>()
				.HasIndex(e => new { e.Host, e.Port })
				.IsUnique();
			modelBuilder.Entity<SpiderProxyEntity>()
				.Ignore(e => e.Address)
				.Ignore(e => e.Value);
		}
	}
}