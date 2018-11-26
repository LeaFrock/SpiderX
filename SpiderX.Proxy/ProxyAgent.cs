using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

		public IEnumerable<SpiderProxyEntity> SelectProxyEntities(Expression<Func<SpiderProxyEntity, bool>> predicate, int count = 0)
		{
			using (var context = new ProxyDbContext(DbConfig))
			{
				return count > 0
					? context.ProxyEntity.Where(predicate).OrderByDescending(e => e.UpdateTime).Take(count)
					: context.ProxyEntity.Where(predicate);
			}
		}

		public int InsertProxyEntities(IEnumerable<SpiderProxyEntity> entities)
		{
			int count = 0;
			var distinctEntities = entities.Distinct(SpiderProxyEntityComparer.Default);
			using (var context = new ProxyDbContext(DbConfig))
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
			using (var context = new ProxyDbContext(DbConfig))
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
			using (var context = new ProxyDbContext(DbConfig))
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
			using (var context = new ProxyDbContext(DbConfig))
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
			using (var context = new ProxyDbContext(DbConfig))
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