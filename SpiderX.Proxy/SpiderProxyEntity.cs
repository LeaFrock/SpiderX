using System;
using System.Globalization;
using System.Net;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxyEntity
	{
		public int Id { get; set; }

		public string Host { get; set; }

		public int Port { get; set; }

		public string Location { get; set; }

		/// <summary>
		/// IP类型（0Http 1Https）
		/// </summary>
		public int Category { get; set; }

		/// <summary>
		/// 匿名度（0透明 1普匿2 混淆 3高匿）
		/// </summary>
		public int AnonymityDegree { get; set; }

		public int ResponseMilliseconds { get; set; }

		private Uri _address;

		public Uri Address
		{
			get
			{
				if (_address == null)
				{
					_address = new Uri("http://" + Host + ':' + Port.ToString(CultureInfo.InvariantCulture));
				}
				return _address;
			}
		}

		private SpiderProxy _value;

		public IWebProxy Value
		{
			get
			{
				if (_value == null)
				{
					_value = new SpiderProxy(Address);
				}
				return _value;
			}
		}
	}
}