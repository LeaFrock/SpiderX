using System;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxyUriEntity : IEquatable<SpiderProxyUriEntity>
	{
		public int Id { get; set; }

		public string Host { get; set; }

		public int Port { get; set; }

		public string Location { get; set; }

		/// <summary>
		/// IP类型（0Http 1Https）
		/// </summary>
		public byte Category { get; set; }

		/// <summary>
		/// 匿名度（0透明 1普匿 2混淆 3高匿）
		/// </summary>
		public byte AnonymityDegree { get; set; }

		public int ResponseMilliseconds { get; set; } = 10000;

		public DateTime UpdateTime { get; set; }

		public string Source { get; set; }

		private Uri _value;

		public Uri Value
		{
			get
			{
				if (_value == null)
				{
					_value = new Uri($"http://{Host}:{Port.ToString()}", UriKind.Absolute);
				}
				return _value;
			}
		}

		public bool Equals(SpiderProxyUriEntity other) => other != null && Host == other.Host && Port == other.Port;

		public override int GetHashCode() => (Host, Port).GetHashCode();
	}
}