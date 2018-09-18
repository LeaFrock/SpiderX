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

		public DbEnum Type { get; set; }

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
			string connection = source.GetSection(nameof(ConnectionString)).Value?.Trim();
			if (string.IsNullOrEmpty(connection))
			{
				return null;
			}
			string typeStr = source.GetSection(nameof(Type)).Value;
			if (!Enum.TryParse(typeStr, out DbEnum type))
			{
				return null;
			}
			instance.Type = type;
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