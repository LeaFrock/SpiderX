using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using SpiderX.BusinessBase;

namespace SpiderX.Launcher
{
	public sealed class CaseSetting : IEquatable<CaseSetting>
	{
		internal CaseSetting(string nameSpace, string caseName, string[] caseParams, int version)
		{
			Namespace = nameSpace ?? throw new ArgumentNullException(nameof(Namespace));
			CaseName = caseName ?? throw new ArgumentNullException(nameof(CaseName));
			Params = caseParams ?? Array.Empty<string>();
			Version = version;
		}

		internal string[] Params { get; }

		internal string Namespace { get; }

		public string CaseName { get; }

		public int Version { get; }

		public string FullTypeName => Namespace + '.' + CaseName;

		public void InvokeCase()
		{
			Type bllType;
			try
			{
				Assembly a = Assembly.Load(Namespace);
				bllType = a.GetType(FullTypeName, false, true);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			if (bllType == null)
			{
				return;
			}
			if (bllType.IsAbstract || bllType.IsNotPublic || !bllType.IsSubclassOf(typeof(BllBase)))
			{
				throw new TypeAccessException("Invalid Type: " + FullTypeName);
			}
			//Create Instance
			BllBase bllInstance;
			try
			{
				bllInstance = (BllBase)Activator.CreateInstance(bllType, false);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			//Invoke Method
			if (Params.Length < 1)
			{
				bllInstance.Run();
			}
			else
			{
				bllInstance.Run(Params);
			}
		}

		public bool Equals(CaseSetting other)
		{
			if (Namespace != other.Namespace)
			{
				return false;
			}
			if (CaseName.EndsWith("bll", StringComparison.CurrentCultureIgnoreCase))
			{
				return CaseName.Contains(other.CaseName, StringComparison.CurrentCultureIgnoreCase);
			}
			return other.CaseName.Contains(CaseName, StringComparison.CurrentCultureIgnoreCase);
		}

		public static CaseSetting CreateDefault(string caseName)
		{
			return CreateDefault(caseName, Array.Empty<string>());
		}

		public static CaseSetting CreateDefault(string caseName, params string[] caseParams)
		{
			return new CaseSetting(GlobalSetting.DefaultNamespace, caseName, caseParams, 0);
		}

		public static CaseSetting FromConfiguration(IConfiguration conf)
		{
			string nameSpace = conf.GetSection(nameof(Namespace)).Value?.Trim();
			if (string.IsNullOrEmpty(nameSpace))
			{
				return null;
			}
			string caseName = conf.GetSection(nameof(CaseName)).Value;
			if (string.IsNullOrWhiteSpace(caseName))
			{
				return null;
			}
			string finalCaseName = CorrectCaseName(caseName);
			string[] caseParams = conf.GetSection(nameof(Params)).GetChildren().Select(p => p.Value).ToArray();
			string vesionStr = conf.GetSection(nameof(Version)).Value?.Trim();
			int version = 0;
			if (!string.IsNullOrEmpty(vesionStr))
			{
				int.TryParse(vesionStr, out version);
			}
			return new CaseSetting(nameSpace, finalCaseName, caseParams, version);
		}

		public static List<CaseSetting> GetListByStringCmd(string cmd)
		{
			string text = cmd.Trim();
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentException("Invalid Command");
			}

			string[] parts = text.Split(';');
			List<CaseSetting> settings = new List<CaseSetting>(parts.Length);
			string trimedPart;
			foreach (string part in parts)
			{
				trimedPart = part.Trim();
				if (trimedPart == string.Empty)
				{
					continue;
				}
				if (CheckSkipStringArg(trimedPart))
				{
					continue;
				}
				if (!trimedPart.Contains(' '))
				{
					settings.Add(CreateDefault(CorrectCaseName(trimedPart)));
					continue;
				}
				string[] tempAry = trimedPart.Split(' ');
                string caseName = CorrectCaseName(tempAry[0]);
                string[] caseParams = new string[tempAry.Length - 1];
				Array.Copy(tempAry, 1, caseParams, 0, caseParams.Length);
				var setting = CreateDefault(caseName, caseParams);
				settings.Add(setting);
			}
			return settings;
		}

		public static List<CaseSetting> GetListByStringArgs(string[] args)
		{
			string cmd = string.Join(' ', args);
			return GetListByStringCmd(cmd);
		}

		private static string CorrectCaseName(string name)
		{
			string trimName = name.Trim();
			if (!trimName.EndsWith("Bll", StringComparison.CurrentCultureIgnoreCase))
			{
				trimName += "Bll";
			}
			return trimName;
		}

		private static bool CheckSkipStringArg(string arg)
		{
			return arg.StartsWith('-') || arg.StartsWith('/');
		}
	}
}