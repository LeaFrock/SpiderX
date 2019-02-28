using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SpiderX.DataClient;

namespace SpiderX.Proxy
{
	public sealed class ProxyAgent<TContext> : IProxyAgent where TContext : ProxyDbContext
	{
		private ProxyAgent()
		{
		}

		public static ProxyAgent<TContext> CreateInstance(DbConfig conf, Func<DbConfig, TContext> dbContextFactory)
		{
			ProxyAgent<TContext> instance = new ProxyAgent<TContext>()
			{
				DbConfig = conf,
				_dbContextCreateFunc = dbContextFactory
			};
			return instance;
		}

		public static ProxyAgent<TContext> CreateInstance(string confName, bool isTest, Func<DbConfig, TContext> dbContextFactory)
		{
			var conf = DbClient.FindConfig(confName, isTest);
			if (conf == null)
			{
				throw new DbConfigNotFoundException(confName);
			}
			return CreateInstance(conf, dbContextFactory);
		}

		private Func<DbConfig, TContext> _dbContextCreateFunc;

		public DbConfig DbConfig { get; private set; }

		public ICollection<SpiderProxyUriEntity> SelectProxyEntities(Predicate<SpiderProxyUriEntity> predicate, int recentDays = 10, int count = 0)
		{
			if (recentDays < 1)
			{
				recentDays = 360;
			}
			Expression<Func<SpiderProxyUriEntity, bool>> filter = predicate != null
				? (p => EF.Functions.DateDiffDay(p.UpdateTime, DateTime.UtcNow) <= recentDays && predicate(p))
				: (Expression<Func<SpiderProxyUriEntity, bool>>)(p => EF.Functions.DateDiffDay(p.UpdateTime, DateTime.UtcNow) <= recentDays);
			using (var context = _dbContextCreateFunc(DbConfig))
			{
				var query = context.ProxyUriEntities
					   .Where(filter)
					   .OrderByDescending(e => e.UpdateTime);
				return (count > 0 ? query.Take(count) : query).ToArray();
			}
		}

		public int InsertProxyEntities(IEnumerable<SpiderProxyUriEntity> entities)
		{
			int count = 0;
			var distinctEntities = entities.Distinct(SpiderProxyEntityComparer.Default);
			using (var context = _dbContextCreateFunc(DbConfig))
			{
				foreach (var entity in distinctEntities)
				{
					if (!context.ProxyUriEntities.Any(p => p.Port == entity.Port && p.Host == entity.Host))
					{
						context.ProxyUriEntities.Add(entity);
						count++;
					}
				}
				return count > 0 ? context.SaveChanges() : 0;
			}
		}

		public int UpdateProxyEntity(int id, Action<SpiderProxyUriEntity> update)
		{
			using (var context = _dbContextCreateFunc(DbConfig))
			{
				var entity = context.ProxyUriEntities.Find(id);
				if (entity != null)
				{
					update(entity);
					return context.SaveChanges();
				}
			}
			return 0;
		}

		public int UpdateProxyEntities(IEnumerable<int> ids, Action<SpiderProxyUriEntity> update)
		{
			var distinctIds = ids.Distinct();
			using (var context = _dbContextCreateFunc(DbConfig))
			{
				foreach (var id in distinctIds)
				{
					var entity = context.ProxyUriEntities.Find(id);
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
			using (var context = _dbContextCreateFunc(DbConfig))
			{
				var entity = context.ProxyUriEntities.FirstOrDefault(p => p.Host == host && p.Port == port);
				if (entity != null)
				{
					context.ProxyUriEntities.Remove(entity);
					return context.SaveChanges();
				}
			}
			return 0;
		}

		public int DeleteProxyEntity(int id)
		{
			using (var context = _dbContextCreateFunc(DbConfig))
			{
				var entity = context.ProxyUriEntities.Find(id);
				if (entity != null)
				{
					context.ProxyUriEntities.Remove(entity);
					return context.SaveChanges();
				}
			}
			return 0;
		}
	}
}