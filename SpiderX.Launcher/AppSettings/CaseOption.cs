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

		public void InitByCommandLine(string commandLineParam)
		{
			string[] parts = commandLineParam.Split('-', StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 1)
			{
				CaseName = CorrectCaseName(parts[0]);
			}
			else
			{
				if (parts[0] != "*")
				{
					NameSpace = parts[0];
				}
				CaseName = CorrectCaseName(parts[1]);
				if (parts.Length > 2)
				{
					if (parts[2] != "_")
					{
						Params = parts[2].Split(',');
					}
					if (parts.Length > 3)
					{
						if (int.TryParse(parts[3], out int version))
						{
							Version = version;
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