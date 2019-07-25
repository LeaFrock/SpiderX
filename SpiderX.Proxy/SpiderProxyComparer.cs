using System.Collections.Generic;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxyEntityComparer : IEqualityComparer<SpiderProxyUriEntity>
	{
		public static IEqualityComparer<SpiderProxyUriEntity> Default { get; } = new SpiderProxyEntityComparer();

		public bool Equals(SpiderProxyUriEntity x, SpiderProxyUriEntity y)
		{
			return x.Port == y.Port && x.Host == y.Host;
		}

		public int GetHashCode(SpiderProxyUriEntity obj) => obj.GetHashCode();
	}
}