using System.Threading;
using Microsoft.Extensions.Configuration;

namespace SpiderX.DataClient
{
	public sealed class DbConfig
	{
		private static int _globalTempId = 0;

		public int TempId { get; set; }

		public string Name { get; set; }

		public string ConnectionString { get; set; }

		public bool IsTest { get; set; } = true;

		public static DbConfig CreateInstance(IConfiguration source)
		{
			DbConfig instance = new DbConfig();
			string enabledStr = source.GetSection("IsEnabled").Value;
			if (!bool.TryParse(enabledStr, out bool isEnabled) || !isEnabled)
			{
				return null;
			}
			string connection = source.GetSection(nameof(ConnectionString)).Value;
			if (string.IsNullOrWhiteSpace(connection))
			{
				return null;
			}
			string testStr = source.GetSection(nameof(IsTest)).Value;
			if (bool.TryParse(testStr, out bool isTest))
			{
				instance.IsTest = isTest;
			}
			instance.Name = source.GetSection(nameof(Name)).Value ?? "???";
			instance.ConnectionString = connection;
			instance.TempId = Interlocked.Increment(ref _globalTempId);
			return instance;
		}
	}
}