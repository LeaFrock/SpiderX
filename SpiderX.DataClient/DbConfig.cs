using System;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace SpiderX.DataClient
{
	public sealed class DbConfig
	{
		private static int _globalTempId = 0;

		public int TempId { get; set; }

		public string Name { get; set; }

		public DbCategoryEnum Cat { get; set; }

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
			string catStr = source.GetSection(nameof(Cat)).Value;
			if (!Enum.TryParse(catStr, out DbCategoryEnum cat))
			{
				return null;
			}
			string testStr = source.GetSection(nameof(IsTest)).Value;
			if (bool.TryParse(testStr, out bool isTest))
			{
				instance.IsTest = isTest;
			}
			instance.Name = source.GetSection(nameof(Name)).Value ?? "???";
			instance.Cat = cat;
			instance.ConnectionString = connection;
			instance.TempId = Interlocked.Increment(ref _globalTempId);
			return instance;
		}
	}

	public enum DbCategoryEnum : byte
	{
		MySql = 0,
		SqlServer = 1
	}
}