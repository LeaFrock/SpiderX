using System;
using Microsoft.EntityFrameworkCore;
using SpiderX.DataClient;

namespace SpiderX.Proxy
{
	public sealed class SqlServerProxyDbContext : ProxyDbContext
	{
		public SqlServerProxyDbContext() : this(null)
		{
		}

		public SqlServerProxyDbContext(DbConfig dbConfig) : base(dbConfig)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			if (Config == null)
			{
				optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=SpiderProxy;Integrated Security=true;Trusted_Connection=True;",
					opt =>
					{
						opt.CommandTimeout(60);
						opt.EnableRetryOnFailure(3);
					});
			}
			else
			{
				switch (Config.Type)
				{
					case DbEnum.SqlServer:
						optionsBuilder.UseSqlServer(Config.ConnectionString,
							opt =>
							{
								opt.CommandTimeout(60);
								opt.EnableRetryOnFailure(3);
							});
						break;

					default:
						throw new NotSupportedException(Config.Name.ToString());
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
				.HasDefaultValueSql("GETUTCDATE()")
				.IsRequired();
				e.Ignore(p => p.Value);
			});
		}
	}
}