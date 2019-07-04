using System;
using System.Collections.Generic;

namespace SpiderX.BusinessBase
{
	public sealed class BllCaseBuildOption
	{
		public static BllCaseBuildOption None { get; } = new BllCaseBuildOption();

		public BllCaseBuildOption() : this(null, 0)
		{
		}

		public BllCaseBuildOption(IReadOnlyList<string> runSettings, int version)
		{
			RunSettings = runSettings ?? Array.Empty<string>();
			Version = version;
		}

		public IReadOnlyList<string> RunSettings { get; }

		public int Version { get; }
	}
}