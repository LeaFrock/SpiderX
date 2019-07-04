using System.Threading;
using Microsoft.Extensions.Configuration;

namespace SpiderX.DataClient
{
	public sealed class DbConfig
	{
		private static int _globalTempId = 0;

		private DbConfig()
		{
			TempId = Interlocked.Increment(ref _globalTempId);
		}

		public DbConfig(string name, DbEnum type, string connStr, bool isTest) : this()
		{
			Name = name;
			Type = type;
			ConnectionString = connStr;
			IsTest = isTest;
		}

		public int TempId { get; }

		public string Name { get; private set; }

		public DbEnum Type { get; private set; }

		public string ConnectionString { get; private set; }

		public bool IsTest { get; private set; } = true;

		public static DbConfig FromConfiguration(IConfiguration source)
		{
			bool isEnabled = source.GetValue<bool>("IsEnabled");
			if (!isEnabled)
			{
				return null;
			}
			string connection = source.GetValue<string>(nameof(ConnectionString))?.Trim();
			if (string.IsNullOrEmpty(connection))
			{
				return null;
			}
			string name = source.GetValue<string>(nameof(Name)) ?? "???";
			var type = source.GetValue<DbEnum>(nameof(Type));
			bool isTest = source.GetValue<bool>(nameof(IsTest));
			return new DbConfig(name, type, connection, isTest);
		}
	}
}