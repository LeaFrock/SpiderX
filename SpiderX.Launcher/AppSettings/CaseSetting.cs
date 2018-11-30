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
			Type bllType = typeof(BllBase);
			if (!bllType.IsClass || bllType.IsAbstract || bllType.IsNotPublic)
			{
				throw new TypeAccessException("Invalid Type: " + CaseName);
			}
			//Create Instance
			object bllInstance;
			try
			{
				bllInstance = Activator.CreateInstance(bllType, false);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			//Invoke Method
			string methodName = nameof(BllBase.Run);
			if (Params.Length < 1)
			{
				MethodInfo mi = bllType.GetMethod(methodName, Type.EmptyTypes);
				if (mi == null)
				{
					throw new MissingMethodException(CaseName + " Method() Not Found.");
				}
				mi.Invoke(bllInstance, null);
			}
			else
			{
				MethodInfo mi = bllType.GetMethod(methodName, new Type[] { typeof(string[]) });
				if (mi == null)
				{
					throw new MissingMethodException(CaseName + " Method(string[]) Not Found.");
				}
				mi.Invoke(bllInstance, new object[] { Params });
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

		public static CaseSetting FromConfiguration(IConfiguration source)
		{
			string nameSpace = source.GetSection(nameof(Namespace)).Value;
			if (string.IsNullOrEmpty(nameSpace))
			{
				return null;
			}
			string caseName = source.GetSection(nameof(CaseName)).Value;
			if (string.IsNullOrEmpty(caseName))
			{
				return null;
			}
			string finalCaseName = CorrectCaseName(caseName);
			string[] caseParams = source.GetSection(nameof(Params)).GetChildren().Select(p => p.Value).ToArray();
			string vesionStr = source.GetSection(nameof(Version)).Value;
			int version = 0;
			if (!string.IsNullOrEmpty(vesionStr))
			{
				int.TryParse(vesionStr, out version);
			}
			return new CaseSetting(nameSpace, finalCaseName, caseParams, version);
		}

		public static List<CaseSetting> GetListByStringArgs(IList<string> args)
		{
			return null;
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
	}
}