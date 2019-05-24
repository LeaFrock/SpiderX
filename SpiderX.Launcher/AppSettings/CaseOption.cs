using System;

namespace SpiderX.Launcher
{
	public sealed class CaseOption
	{
		public string NameSpace { get; set; }

		public string CaseName { get; set; }

		public string[] Params { get; set; }

		public int Version { get; set; }

		public string FullTypeName => NameSpace + '.' + CaseName;

		public static void InitOption(CaseOption option, string commandLineParam)
		{
			string[] parts = commandLineParam.Split('-', StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 1)
			{
				option.CaseName = CorrectCaseName(parts[0]);
			}
			else
			{
				if (parts[0] != "*")
				{
					option.NameSpace = parts[0];
				}
				option.CaseName = parts[1];
				if (parts.Length > 2)
				{
					if (parts[2] != "_")
					{
						option.Params = parts[2].Split(',');
					}
					if (parts.Length > 3)
					{
						if (int.TryParse(parts[3], out int version))
						{
							option.Version = version;
						}
					}
				}
			}
		}

		public static bool CheckSkipStringArg(string arg)
		{
			return arg.StartsWith('/');
		}

		private static string CorrectCaseName(string name)
		{
			return name.EndsWith("Bll", StringComparison.CurrentCultureIgnoreCase) ? name : name + "Bll";
		}
	}
}