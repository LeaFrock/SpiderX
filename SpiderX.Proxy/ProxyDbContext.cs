using System;
using Microsoft.EntityFrameworkCore;
using SpiderX.DataClient;

namespace SpiderX.Proxy
{
	public sealed class ProxyDbContext : DbContext
	{
		public ProxyDbContext() : base()
		{
		}

		public ProxyDbContext(DbConfig dbConfig) : this()
		{
			Config = dbConfig;
		}

		public DbConfig Config { get; }

		public DbSet<SpiderProxyEntity> ProxyEntities { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			if (Config == null)
			{
				optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=SpiderProxy;Integrated Security=true;Trusted_Connection=True;");
			}
			else
			{
				switch (Config.Type)
				{
					case DbEnum.SqlServer:
						optionsBuilder.UseSqlServer(Config.ConnectionString);
						break;

					case DbEnum.MySql:
						optionsBuilder.UseMySql(Config.ConnectionString);
						break;

					default:
						throw new NotSupportedException(Config.Type.ToString());
				}
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<SpiderProxyEntity>()
				.HasKey(e => e.Id);
			modelBuilder.Entity<SpiderProxyEntity>()
				.Property(e => e.Id)
				.ValueGeneratedOnAdd();
			modelBuilder.Entity<SpiderProxyEntity>()
				.Property(e => e.Host)
				.HasColumnType("VARCHAR(32)");
			modelBuilder.Entity<SpiderProxyEntity>()
				.HasIndex(e => new { e.Host, e.Port })
				.IsUnique();
			modelBuilder.Entity<SpiderProxyEntity>()
				.Ignore(e => e.Address)
				.Ignore(e => e.Value);
		}
	}
}