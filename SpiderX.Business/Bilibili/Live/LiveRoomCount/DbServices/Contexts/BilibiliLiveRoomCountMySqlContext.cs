using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SpiderX.DataClient;

namespace SpiderX.Business.Bilibili
{
	public partial class BilibiliLiveRoomCountBll
	{
		public sealed class BilibiliLiveRoomCountMySqlContext : BilibiliLiveRoomCountContext
		{
			private readonly static DbConfig _defaultDbConfig = new DbConfig(
				"DefaultMySql",
				DbEnum.MySql,
				@"User ID=root;Password=20180423;Host=localhost;Port=3306;Database=BiliBili;Protocol=TCP;Compress=false;Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=;",
				true);

			public BilibiliLiveRoomCountMySqlContext() : this(_defaultDbConfig)
			{
			}

			public BilibiliLiveRoomCountMySqlContext(DbConfig config) : base(config)
			{
			}

			protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				base.OnModelCreating(modelBuilder);
				modelBuilder.Entity<BilibiliLiveRoomCount>(
					e =>
					{
						e.HasKey(p => p.Id);
						e.Property(p => p.Id)
						.ValueGeneratedOnAdd();
						e.HasIndex(p => new { p.Month, p.Day, p.Hour })
						.IsUnique()
						.HasName("DateKey");
						e.HasIndex(p => p.Month);
						e.ToTable("LiveRoomCount_" + DateTime.Now.Year.ToString());
					});
			}

			protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			{
				base.OnConfiguring(optionsBuilder);
				switch (Config.Type)
				{
					case DbEnum.MySql:
						optionsBuilder.UseMySql(Config.ConnectionString, opt =>
						{
							opt.CommandTimeout(60);
							opt.EnableRetryOnFailure(3);
						});
						break;

					default:
						throw new NotSupportedException(Config.Name);
				}
			}
		}
	}
}