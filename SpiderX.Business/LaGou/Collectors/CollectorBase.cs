using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiderX.Http.Util;
using SpiderX.Proxy;

namespace SpiderX.Business.LaGou
{
	public sealed partial class LaGouBll
	{
		private abstract class CollectorBase
		{
			public abstract Task<LaGouResponseDataCollection> CollectAsync(LaGouSearchParam searchParam);

			protected virtual IProxyUriLoader CreateProxyUriLoader()
			{
				DefaultProxyUriLoader loader = new DefaultProxyUriLoader()
				{
					Days = 360,
					DbContextFactory = () => ProxyDbContext.CreateInstance(),
					EntityOption = new SpiderProxyUriEntityOption() { Category = 1, AnonymityDegree = 3, ResponseMilliseconds = 10000 }
				};
				return loader;
			}
		}
	}
}