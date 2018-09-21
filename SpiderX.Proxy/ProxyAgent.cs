using System;
using System.Collections.Generic;
using System.Linq;
using SpiderX.DataClient;

namespace SpiderX.Proxy
{
	public sealed class ProxyAgent
	{
		public ProxyAgent(DbConfig conf)
		{
			DbConfig = conf;
		}

		public DbConfig DbConfig { get; }

		public List<SpiderProxyEntity> SelectTotalProxyEntities()
		{
			using (var context = new ProxyDbContext(DbConfig))
			{
				return context.ProxyEntities.ToList();
			}
		}

		public List<SpiderProxyEntity> SelectProxyEntities(Func<SpiderProxyEntity, bool> match)
		{
			using (var context = new ProxyDbContext(DbConfig))
			{
				return context.ProxyEntities.Where(match).ToList();
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
					if (!context.ProxyEntities.Any(p => p.Host == entity.Host && p.Port == entity.Port))
					{
						context.ProxyEntities.Add(entity);
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
				var entity = context.ProxyEntities.Find(id);
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
					var entity = context.ProxyEntities.Find(id);
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
				var entity = context.ProxyEntities.FirstOrDefault(p => p.Host == host && p.Port == port);
				if (entity != null)
				{
					context.ProxyEntities.Remove(entity);
					return context.SaveChanges();
				}
			}
			return 0;
		}

		public int DeleteProxyEntity(int id)
		{
			using (var context = new ProxyDbContext(DbConfig))
			{
				var entity = context.ProxyEntities.Find(id);
				if (entity != null)
				{
					context.ProxyEntities.Remove(entity);
					return context.SaveChanges();
				}
			}
			return 0;
		}
	}
}