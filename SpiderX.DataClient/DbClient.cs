using System.Threading;

namespace SpiderX.DataClient
{
	public sealed class DbClient
	{
		private DbClientSetting _setting;

		public DbClientSetting Setting
		{
			get
			{
				if (_setting == null)
				{
					Interlocked.CompareExchange(ref _setting, DbClientSetting.Instance, null);
				}
				return _setting;
			}
			set
			{
				Interlocked.Exchange(ref _setting, value);
			}
		}

		//public IDbConnection CreateConnection(string connectString)
		//{
		//}
	}
}