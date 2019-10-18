using System;
using System.Collections.Generic;

namespace SpiderX.BusinessBase
{
	public sealed class BllCaseBuildOption
	{
		public static BllCaseBuildOption None { get; } = new BllCaseBuildOption();

		public BllCaseBuildOption() : this(null, null, 0)
		{
		}

		public BllCaseBuildOption(IReadOnlyList<string> runSettings, string dbConfigName, int version)
		{
			RunSettings = runSettings ?? Array.Empty<string>();
			DbConfigName = dbConfigName;
			Version = version;
		}

		public IReadOnlyList<string> RunSettings { get; }

		public string DbConfigName { get; }

		public int Version { get; }
	}
}