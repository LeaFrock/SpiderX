using System;
using System.Collections.Generic;
using SpiderX.DataClient;

namespace SpiderX.Proxy
{
	public interface IProxyAgent
	{
		DbConfig DbConfig { get; }

		ICollection<SpiderProxyUriEntity> SelectProxyEntities(Predicate<SpiderProxyUriEntity> predicate, int recentDays, int count);

		int InsertProxyEntities(IEnumerable<SpiderProxyUriEntity> entities);

		int UpdateProxyEntity(int id, Action<SpiderProxyUriEntity> update);

		int UpdateProxyEntities(IEnumerable<int> ids, Action<SpiderProxyUriEntity> update);

		int DeleteProxyEntity(int id);

		int DeleteProxyEntity(string host, int port);
	}
}