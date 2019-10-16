using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
						throw new NotSupportedException(Config.Name);
				}
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
				e.Property(p => p.UpdateTime)
				.HasColumnType("DATETIME")
				.HasDefaultValueSql("GETUTCDATE()")
				.IsRequired();
				e.Ignore(p => p.Value);
			});
		}

		public override ICollection<SpiderProxyUriEntity> SelectProxyEntities(Predicate<SpiderProxyUriEntity> predicate, int recentDays = 10, int count = 0)
		{
			if (recentDays < 1)
			{
				recentDays = 360;
			}
			Expression<Func<SpiderProxyUriEntity, bool>> filter = predicate != null
				? (p => SqlServerDbFunctionsExtensions.DateDiffDay(EF.Functions, p.UpdateTime, DateTime.UtcNow) <= recentDays && predicate(p))
				: (Expression<Func<SpiderProxyUriEntity, bool>>)(p => SqlServerDbFunctionsExtensions.DateDiffDay(EF.Functions, p.UpdateTime, DateTime.UtcNow) <= recentDays);
			var query = ProxyUriEntities
				.Where(filter)
				.OrderByDescending(e => e.UpdateTime);
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