using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SpiderX.DataClient;

namespace SpiderX.Business.Bilibili
{
	internal sealed class BilibiliDbContext : DbContext
	{
		public BilibiliDbContext(DbConfig config) : base()
		{
			Config = config;
		}

		public DbConfig Config { get; }

		internal DbSet<BilibiliLiveRoomCount> LiveRoomCount { get; set; }

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