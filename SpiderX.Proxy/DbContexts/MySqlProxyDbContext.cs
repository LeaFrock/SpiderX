using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SpiderX.DataClient;

namespace SpiderX.Proxy
{
	public sealed class MySqlProxyDbContext : ProxyDbContext
	{
		public MySqlProxyDbContext(DbConfig dbConfig, string dbName) : base(dbConfig, dbName)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			switch (Config.Type)
			{
				case DbEnum.MySql:
					string connStr = Config.ConnectionString;
					if (Config.IsConnectionStringTemplate)
					{
						connStr = string.Format(connStr, DbName);
					}
					optionsBuilder.UseMySql(connStr,
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

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<SpiderProxyUriEntity>(e =>
			{
				e.HasKey(p => p.Id);
				e.Property(p => p.Id)
				.ValueGeneratedOnAdd();
				e.HasIndex(p => new { p.Host, p.Port })
				.IsUnique();
				e.Property(p => p.Host)
				.HasColumnType("VARCHAR(32)")
				.IsRequired();
				e.Property(p => p.Location)
				.HasColumnType("VARCHAR(32)")
				.IsRequired();
				e.Property(p => p.Source)
				.HasColumnType("VARCHAR(25)")
				.IsRequired();
				e.Property(p => p.UpdateTime)
				.HasColumnType("DATETIME")
				.HasDefaultValueSql("CURRENT_TIMESTAMP()")
				.IsRequired();
				e.Ignore(p => p.Value);
				e.ToTable("ProxyEntities");
			});
		}

		public override ICollection<SpiderProxyUriEntity> SelectProxyEntities(ISpiderProxyUriEntityOption entityOption, int recentDays = 10, int count = 0)
		{
			if (recentDays < 1)
			{
				recentDays = 360;
			}
			IQueryable<SpiderProxyUriEntity> query;
			if (entityOption is null)
			{
				query = ProxyUriEntities.AsNoTracking()
					.Where(p => MySqlDbFunctionsExtensions.DateDiffDay(EF.Functions, p.UpdateTime, DateTime.UtcNow) <= recentDays)
					.OrderByDescending(e => e.UpdateTime);
			}
			else
			{
				var queryableData = ProxyUriEntities.AsNoTracking().AsQueryable();
				var expression = entityOption.GetExpressionTree(queryableData);
				if (expression is null)
				{
					query = ProxyUriEntities
					.Where(p => MySqlDbFunctionsExtensions.DateDiffDay(EF.Functions, p.UpdateTime, DateTime.UtcNow) <= recentDays)
					.OrderByDescending(e => e.UpdateTime);
				}
				else
				{
					query = queryableData.Provider
						.CreateQuery<SpiderProxyUriEntity>(expression)
						.Where(p => MySqlDbFunctionsExtensions.DateDiffDay(EF.Functions, p.UpdateTime, DateTime.UtcNow) <= recentDays)
						.OrderByDescending(e => e.UpdateTime);
				}
			}
			return (count > 0 ? query.Take(count) : query).ToArray();
		}

		public override int InsertProxyEntities(IEnumerable<SpiderProxyUriEntity> entities)
		{
			int count = 0;
			var distinctEntities = entities.Distinct(SpiderProxyEntityComparer.Default);
			foreach (var entity in distinctEntities)
			{
				if (!ProxyUriEntities.Any(p => p.Port == entity.Port && p.Host == entity.Host))
				{
					ProxyUriEntities.Add(entity);
					count++;
				}
			}
			return count > 0 ? SaveChanges() : 0;
		}

		public override int UpdateProxyEntity(int id, Action<SpiderProxyUriEntity> updateAction)
		{
			var entity = ProxyUriEntities.Find(id);
			if (entity != null)
			{
				updateAction(entity);
				return SaveChanges();
			}
			return 0;
		}

		public override int UpdateProxyEntities(IEnumerable<int> ids, Action<SpiderProxyUriEntity> updateAction)
		{
			var distinctIds = ids.Distinct();
			foreach (var id in distinctIds)
			{
				var entity = ProxyUriEntities.Find(id);
				if (entity != null)
				{
					updateAction(entity);
				}
			}
			return SaveChanges();
		}

		public override int DeleteProxyEntity(string host, int port)
		{
			var entity = ProxyUriEntities.FirstOrDefault(p => p.Host == host && p.Port == port);
			if (entity != null)
			{
				ProxyUriEntities.Remove(entity);
				return SaveChanges();
			}
			return 0;
		}

		public override int DeleteProxyEntity(int id)
		{
			var entity = ProxyUriEntities.Find(id);
			if (entity != null)
			{
				ProxyUriEntities.Remove(entity);
				return SaveChanges();
			}
			return 0;
		}
	}
}