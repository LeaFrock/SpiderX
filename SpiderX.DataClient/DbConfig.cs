using System;
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
			ConnectionString = connStr ?? throw new ArgumentNullException("ConnectionString cannot be null.");
			IsConnectionStringTemplate = connStr.Contains('{');
			Name = name;
			Type = type;
			IsTest = isTest;
		}

		public int TempId { get; }

		public string Name { get; }

		public DbEnum Type { get; }

		public string ConnectionString { get; }

		public bool IsTest { get; } = true;

		public bool IsConnectionStringTemplate { get; }

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