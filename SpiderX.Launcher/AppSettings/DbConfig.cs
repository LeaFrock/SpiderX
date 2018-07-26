using System;
using System.ComponentModel;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace SpiderX.Launcher
{
	public sealed class DbConfig
	{
		private static int _globalTempId = 0;

		public int TempId { get; set; }

		public string Name { get; set; }

		public DbCategoryEnum Cat { get; set; }

		public string Connection { get; set; }

		[DefaultValue(true)]
		public bool IsTest { get; set; }

		public static DbConfig CreateInstance(IConfiguration source)
		{
			DbConfig instance = new DbConfig();
			string enabledStr = source.GetSection("IsEnabled").Value;
			if (!bool.TryParse(enabledStr, out bool isEnabled) || !isEnabled)
			{
				return null;
			}
			string connection = source.GetSection("ConnectionString").Value;
			if (string.IsNullOrWhiteSpace(connection))
			{
				return null;
			}
			string catStr = source.GetSection("Cat").Value;
			if (!Enum.TryParse(catStr, out DbCategoryEnum cat))
			{
				return null;
			}
			string testStr = source.GetSection("IsTest").Value;
			if (bool.TryParse(testStr, out bool isTest))
			{
				instance.IsTest = isTest;
			}
			instance.Name = source.GetSection("Name").Value ?? "???";
			instance.Cat = cat;
			instance.Connection = connection;
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