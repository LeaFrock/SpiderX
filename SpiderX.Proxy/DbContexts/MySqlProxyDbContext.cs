using System;
using Microsoft.EntityFrameworkCore;
using SpiderX.DataClient;

namespace SpiderX.Proxy
{
	public sealed class MySqlProxyDbContext : ProxyDbContext
	{
		public MySqlProxyDbContext() : this(null)
		{
		}

		public MySqlProxyDbContext(DbConfig dbconfig) : base(dbconfig)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			if (Config == null)
			{
				optionsBuilder.UseMySql(@"Server=localhost;Database=SpiderProxy;UID=root;Password=20180423;CharSet=utf8;pooling=true;port=3306;sslmode=none;",
					opt =>
					{
						opt.CommandTimeout(60);
						opt.EnableRetryOnFailure(3);
						//opt.ServerVersion("8.0.11");
					});
			}
			else
			{
				switch (Config.Type)
				{
					case DbEnum.MySql:
						optionsBuilder.UseMySql(Config.ConnectionString,
							opt =>
							{
								opt.CommandTimeout(60);
								opt.EnableRetryOnFailure(3);
								//opt.ServerVersion("8.0.11");
							});
						break;

					default:
						throw new NotSupportedException(Config.Name);
				}
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<SpiderProxyEntity>(e =>
			{
				e.HasKey(p => p.Id);
				e.Property(p => p.Id)
				.ValueGeneratedOnAdd();
				e.HasIndex(p => new { p.Host, p.Port })
				.IsUnique();
				e.Property(p => p.Host)
				.HasColumnType("VARCHAR(32)")
				.IsRequired();
				e.Property(p => p.UpdateTime)
				.HasColumnType("DATETIME")
				.HasDefaultValueSql("utc_timestamp()")
				.IsRequired();
				e.Ignore(p => p.Value);
			});
		}
	}
}