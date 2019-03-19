using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SpiderX.Business.LaGou.DbEntities;
using SpiderX.DataClient;

namespace SpiderX.Business.LaGou.DbContexts
{
	public sealed class LaGouSqlServerContext : LaGouContext
	{
		public LaGouSqlServerContext() : this(null)
		{
		}

		public LaGouSqlServerContext(DbConfig dbConfig) : base(dbConfig)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (Config == null)
			{
				optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=LaGou;Integrated Security=true;Trusted_Connection=True;",
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
						throw new NotSupportedException(Config.Name);
				}
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<LaGouPositionEntity>(e =>
			{
				e.HasKey(p => p.Id);
				e.Property(p => p.Id)
				.ValueGeneratedOnAdd();
				e.HasIndex(p => p.PositionId)
				.IsUnique();
				e.HasIndex(p => p.CompanyId);
				e.HasIndex(p => p.PublisherId);
				e.HasIndex(p => p.Keyword);
				e.Property(p => p.Name)
				.HasColumnType("VARCHAR(32)")
				.IsRequired();
				e.Property(p => p.Keyword)
				.HasColumnType("VARCHAR(32)")
				.IsRequired();
				e.Property(p => p.CreateTime)
				.HasColumnType("DATETIME");
				e.Property(p => p.DbCreateTime)
				.HasColumnType("DATETIME")
				.HasDefaultValueSql("GETDATE()")
				.IsRequired();
			});

			modelBuilder.Entity<LaGouCompanyEntity>(e =>
			{
				e.HasKey(p => p.Id);
				e.Property(p => p.Id)
				.ValueGeneratedOnAdd();
				e.HasIndex(p => p.CompanyId)
				.IsUnique();
				e.HasIndex(p => p.DistrictName);
				e.Property(p => p.Name)
				.HasColumnType("VARCHAR(32)")
				.IsRequired();
				e.Property(p => p.DbCreateTime)
				.HasColumnType("DATETIME")
				.HasDefaultValueSql("GETDATE()")
				.IsRequired();
				e.Property(p => p.DbLatestUpdateTime)
				.HasColumnType("DATETIME")
				.HasDefaultValueSql("GETDATE()")
				.IsRequired();
			});

			modelBuilder.Entity<LaGouHrInfoEntity>(e =>
			{
				e.HasKey(p => p.Id);
				e.Property(p => p.Id)
				.ValueGeneratedOnAdd();
				e.HasIndex(p => p.UserId)
				.IsUnique();
				e.HasIndex(p => p.CompanyId);
				e.Property(p => p.Name)
				.HasColumnType("VARCHAR(32)")
				.IsRequired();
			});

			modelBuilder.Entity<LaGouHrDailyRecordEntity>(e =>
			{
				e.HasKey(p => p.Id);
				e.Property(p => p.Id)
				.ValueGeneratedOnAdd();
				e.HasIndex(p => p.UserId);
				e.HasIndex(p => p.DateNumber);
				e.Property(p => p.DbCreateTime)
				.HasColumnType("DATETIME")
				.HasDefaultValueSql("GETDATE()")
				.IsRequired();
			});
		}
	}
}