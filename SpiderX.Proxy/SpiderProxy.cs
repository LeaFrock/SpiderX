﻿using System.Net;
using System.Threading;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxy : WebProxy
	{
		public SpiderProxy(string Host, int Port) : base(Host, Port)
		{
		}

		private static WebProxy _local;

		/// <summary>
		/// Use for local web debugging tools, like Fiddler etc.
		/// </summary>
		public static IWebProxy Local
		{
			get
			{
				if (_local == null)
				{
					Interlocked.CompareExchange(ref _local, new WebProxy("127.0.0.1", 8888), null);
				}
				return _local;
			}
		}
	}
}