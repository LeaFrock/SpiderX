using System;
using System.Threading;

namespace SpiderX.DataClient
{
	public sealed class DbClient
	{
		public DbClient() : this(DbClientSetting.Instance)
		{
		}

		public DbClient(DbClientSetting setting)
		{
			Setting = setting ?? throw new ArgumentNullException("Setting Cannot be Null.");
		}

		private static DbClient _default;

		public static DbClient Default
		{
			get
			{
				if (_default == null)
				{
					Interlocked.CompareExchange(ref _default, new DbClient(), null);
				}
				return _default;
			}
		}

		public DbClientSetting Setting { get; }

		public DbConfig FindConfig(string name, bool isTest)
		{
			return Setting.FindConfig(name, isTest);
		}
	}
}