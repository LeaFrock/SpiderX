using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SpiderX.Launcher
{
	public sealed class CaseSetting
	{
		public string NameSpace { get; set; }

		public string CaseName { get; set; }

		public string[] Params { get; set; }

		public int Version { get; set; }

		public string FullTypeName => NameSpace + '.' + CaseName;

		public static CaseSetting FromCommandLine(string commandLineParam, string defaultNamespace)
		{
			var instance = new CaseSetting() { NameSpace = defaultNamespace };
			string[] parts = commandLineParam.Split('-', StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 1)
			{
				instance.CaseName = CorrectCaseName(parts[0]);
			}
			else
			{
				if (parts[0] != "*")
				{
					instance.NameSpace = parts[0];
				}
				instance.CaseName = CorrectCaseName(parts[1]);
				if (parts.Length > 2)
				{
					if (parts[2] != "_")
					{
						instance.Params = parts[2].Split(',');
					}
					if (parts.Length > 3)
					{
						if (int.TryParse(parts[3], out int version))
						{
							instance.Version = version;
						}
					}
				}
			}
			return instance;
		}

		public static CaseSetting FromConfiguration(IConfiguration cs)
		{
			var instance = new CaseSetting()
			{
				NameSpace = cs.GetValue<string>("NameSpace"),
				CaseName = cs.GetValue<string>("CaseName"),
				Version = cs.GetValue<int>("Version"),
			};
			var paramSection = cs.GetSection("Params").GetChildren();
			if (paramSection.Any())
			{
				List<string> paramList = new List<string>();
				foreach (var ps in paramSection)
				{
					paramList.Add(ps.Value);
				}
				instance.Params = paramList.ToArray();
			}
			return instance;
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