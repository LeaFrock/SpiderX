using System;
using System.Collections.Generic;
using SpiderX.DataClient;

namespace SpiderX.Proxy
{
	public interface IProxyAgent
	{
		DbConfig DbConfig { get; }

		ICollection<SpiderProxyEntity> SelectProxyEntities(Predicate<SpiderProxyEntity> predicate, int recentDays, int count);

		int InsertProxyEntities(IEnumerable<SpiderProxyEntity> entities);

		int UpdateProxyEntity(int id, Action<SpiderProxyEntity> update);

		int UpdateProxyEntities(IEnumerable<int> ids, Action<SpiderProxyEntity> update);

		int DeleteProxyEntity(int id);

		int DeleteProxyEntity(string host, int port);
	}
}