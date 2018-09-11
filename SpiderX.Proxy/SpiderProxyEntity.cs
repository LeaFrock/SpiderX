using System;
using System.Globalization;
using System.Net;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxyEntity
	{
		public string Host { get; set; }

		public string Port { get; set; }

		public string Location { get; set; }

		public bool HttpsEnabled { get; set; }

		/// <summary>
		/// 匿名度（0透明1普匿2混淆3高匿）
		/// </summary>
		public int AnonymityDegree { get; set; }

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

		private SpiderProxy _proxy;

		public IWebProxy Proxy
		{
			get
			{
				if (_proxy == null)
				{
					_proxy = new SpiderProxy(Address);
				}
				return _proxy;
			}
		}
	}
}