using System.Collections.Generic;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxyEntityComparer : IEqualityComparer<SpiderProxyUriEntity>
	{
		private static readonly SpiderProxyEntityComparer _default = new SpiderProxyEntityComparer();

		public static IEqualityComparer<SpiderProxyUriEntity> Default => _default;

		public bool Equals(SpiderProxyUriEntity x, SpiderProxyUriEntity y)
		{
			return x.Port == y.Port && x.Host == y.Host;
		}

		public int GetHashCode(SpiderProxyUriEntity obj) => obj.GetHashCode();
	}
}