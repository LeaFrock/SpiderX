using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpiderX.BusinessBase;

namespace SpiderX.Launcher
{
	public static class BllCaseBuilderHelper
	{
		public static IBllCaseBuilder FromCommandLine(string commandLineParam, string defaultNamespace, ILoggerFactory loggerFactory = null)
		{
			string nameSpace = defaultNamespace;
			string caseName;
			string[] runSettings = null;
			int version = 0;
			string[] parts = commandLineParam.Split('-', StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 1)
			{
				caseName = CorrectCaseName(parts[0]);
			}
			else
			{
				if (parts[0] != "*")
				{
					nameSpace = parts[0];
				}
				caseName = CorrectCaseName(parts[1]);
				if (parts.Length > 2)
				{
					if (parts[2] != "_")
					{
						runSettings = parts[2].Split(',');
					}
					if (parts.Length > 3)
					{
						int.TryParse(parts[3], out version);
					}
				}
			}
			var logger = loggerFactory?.CreateLogger(nameSpace + '.' + caseName);
			if (!TryGetCaseType(nameSpace, caseName, logger, out var caseType))
			{
				return EmptyBllCaseBuilder.Default;
			}
			return new DefaultBllCaseBuilder(caseType, new BllCaseBuildOption(runSettings, version), logger);
		}

		public static IBllCaseBuilder FromConfiguration(IConfiguration cs, ILoggerFactory loggerFactory = null)
		{
			string nameSpace = cs.GetValue<string>("NameSpace");
			string caseName = cs.GetValue<string>("CaseName");
			var logger = loggerFactory?.CreateLogger(nameSpace + '.' + caseName);
			if (!TryGetCaseType(nameSpace, caseName, logger, out var caseType))
			{
				return EmptyBllCaseBuilder.Default;
			}
			int version = cs.GetValue<int>("Version");
			string[] runSettings = null;
			var paramSection = cs.GetSection("Params").GetChildren();
			if (paramSection.Any())
			{
				var paramList = new List<string>(4);
				foreach (var ps in paramSection)
				{
					paramList.Add(ps.Value);
				}
				runSettings = paramList.ToArray();
			}
			return new DefaultBllCaseBuilder(caseType, new BllCaseBuildOption(runSettings, version), logger);
		}

		private static bool TryGetCaseType(string nameSpace, string caseName, ILogger logger, out Type caseType)
		{
			string fullTypeName = nameSpace + '.' + caseName;
			try
			{
				Assembly a = Assembly.Load(nameSpace);
				caseType = a.GetType(fullTypeName, true, true);
			}
			catch (Exception ex)
			{
				logger?.LogError(ex, ex.Message);
				caseType = null;
				return false;
			}
			return caseType != null;
		}

		private static string CorrectCaseName(string name)
		{
			return name.EndsWith("Bll", StringComparison.CurrentCultureIgnoreCase) ? name : name + "Bll";
		}
	}
}