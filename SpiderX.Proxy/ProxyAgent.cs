using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SpiderX.DataClient;

namespace SpiderX.Proxy
{
	public class ProxyAgent
	{
		public ProxyAgent(DbConfig conf)
		{
			DbConfig = conf;
		}

		public DbConfig DbConfig { get; }

		public IEnumerable<SpiderProxyEntity> SelectProxyEntities(Func<SpiderProxyEntity, bool> predicate, int recentDays = 10, int count = 0)
		{
			Expression<Func<SpiderProxyEntity, bool>> filter = predicate == null
				? (p => EF.Functions.DateDiffDay(p.UpdateTime, DateTime.UtcNow) <= recentDays && predicate(p))
				: (Expression<Func<SpiderProxyEntity, bool>>)(p => EF.Functions.DateDiffDay(p.UpdateTime, DateTime.UtcNow) <= recentDays);
			using (var context = new SqlServerProxyDbContext(DbConfig))
			{
				var query = context.ProxyEntity
					   .Where(filter)
					   .OrderByDescending(e => e.UpdateTime);
				return count > 0 ? query.Take(count) : query;
			}
		}

		public int InsertProxyEntities(IEnumerable<SpiderProxyEntity> entities)
		{
			int count = 0;
			var distinctEntities = entities.Distinct(SpiderProxyEntityComparer.Default);
			using (var context = new SqlServerProxyDbContext(DbConfig))
			{
				foreach (var entity in distinctEntities)
				{
					if (!context.ProxyEntity.Any(p => p.Host == entity.Host && p.Port == entity.Port))
					{
						context.ProxyEntity.Add(entity);
						count++;
					}
				}
				return count > 0 ? context.SaveChanges() : 0;
			}
		}

		public int UpdateProxyEntity(int id, Action<SpiderProxyEntity> update)
		{
			using (var context = new SqlServerProxyDbContext(DbConfig))
			{
				var entity = context.ProxyEntity.Find(id);
				if (entity != null)
				{
					update(entity);
					return context.SaveChanges();
				}
			}
			return 0;
		}

		public int UpdateProxyEntities(IEnumerable<int> ids, Action<SpiderProxyEntity> update)
		{
			var distinctIds = ids.Distinct();
			using (var context = new SqlServerProxyDbContext(DbConfig))
			{
				foreach (var id in distinctIds)
				{
					var entity = context.ProxyEntity.Find(id);
					if (entity != null)
					{
						update(entity);
					}
				}
				return context.SaveChanges();
			}
		}

		public int DeleteProxyEntity(string host, int port)
		{
			using (var context = new SqlServerProxyDbContext(DbConfig))
			{
				var entity = context.ProxyEntity.FirstOrDefault(p => p.Host == host && p.Port == port);
				if (entity != null)
				{
					context.ProxyEntity.Remove(entity);
					return context.SaveChanges();
				}
			}
			return 0;
		}

		public int DeleteProxyEntity(int id)
		{
			using (var context = new SqlServerProxyDbContext(DbConfig))
			{
				var entity = context.ProxyEntity.Find(id);
				if (entity != null)
				{
					context.ProxyEntity.Remove(entity);
					return context.SaveChanges();
				}
			}
			return 0;
		}
	}
}