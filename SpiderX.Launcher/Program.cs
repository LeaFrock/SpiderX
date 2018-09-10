using System;
using System.IO;
using System.Reflection;
using SpiderX.BusinessBase;
using SpiderX.Extensions;

namespace SpiderX.Launcher
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			//Load SettingManager
			AppSettingManager settingManager = AppSettingManager.Instance;
			//Startup
			StartUp.Run();
			//Get Input
			string caseName;
			string[] caseParams;
			if (args.IsNullOrEmpty())
			{
				caseName = settingManager.CaseName;
				caseParams = settingManager.CaseParams;
			}
			else
			{
				caseName = args[0];
				if (args.Length > 1)
				{
					caseParams = new string[args.Length - 1];
					Array.Copy(args, 1, caseParams, 0, caseParams.Length);
				}
				else
				{
					caseParams = null;
				}
			}
			caseName = CorrectCaseName(caseName);
			//Get TargetType
			if (!TryGetType(caseName, out Type bllType))
			{
				return;
			}
			if (!bllType.IsClass || bllType.IsNotPublic)
			{
				throw new TypeAccessException(settingManager.CaseName + " Invalid Class.");
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
			//Get&Invoke Method
			string methodName = nameof(BllBase.Run);
			if (caseParams.IsNullOrEmpty())
			{
				MethodInfo mi = bllType.GetMethod(methodName);
				if (mi == null)
				{
					throw new MissingMethodException(caseName + " Method() Not Found.");
				}
				mi.Invoke(bllInstance, null);
			}
			else
			{
				MethodInfo mi = bllType.GetMethod(methodName, new Type[] { typeof(string[]) });
				if (mi == null)
				{
					throw new MissingMethodException(caseName + " Method(string[]) Not Found.");
				}
				mi.Invoke(bllInstance, new object[] { caseParams });
			}
			//End
			if (!settingManager.AutoClose)
			{
				Console.ReadKey();
			}
		}

		private static bool TryGetType(string className, out Type caseType)
		{
			Type[] types;
			try
			{
				Assembly a = Assembly.LoadFrom(Path.Combine(StartUp.ModulesDirectoryName, AppSettingManager.Instance.BusinessModuleName));
				types = a.GetTypes();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			caseType = Array.Find(types, t => t.Name.Equals(className, StringComparison.CurrentCultureIgnoreCase));
			return caseType != null;
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