using System.Collections.Generic;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxyEntityComparer : IEqualityComparer<SpiderProxyEntity>
	{
		private static readonly SpiderProxyEntityComparer _default = new SpiderProxyEntityComparer();

		public static IEqualityComparer<SpiderProxyEntity> Default => _default;

		public bool Equals(SpiderProxyEntity x, SpiderProxyEntity y)
		{
			return x.Port == y.Port && x.Host == y.Host;
		}

		public int GetHashCode(SpiderProxyEntity obj)
		{
			return obj.Address.Authority.GetHashCode();
		}
	}
}