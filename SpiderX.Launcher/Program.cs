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
			//Startup
			StartUp startUp = new StartUp();
			startUp.Run();
			//Load SettingManager
			AppSettingManager settingManager = AppSettingManager.Instance;
			//Get TargetType
			if (!TryGetType(settingManager.CaseName, out Type bllType))
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
			if (settingManager.CaseParams.IsNullOrEmpty())
			{
				MethodInfo mi = bllType.GetMethod(methodName);
				if (mi == null)
				{
					throw new MissingMethodException(settingManager.CaseName + " Method() Not Found.");
				}
				mi.Invoke(bllInstance, null);
			}
			else
			{
				MethodInfo mi = bllType.GetMethod(methodName, new Type[] { typeof(string[]) });
				if (mi == null)
				{
					throw new MissingMethodException(settingManager.CaseName + " Method(string[]) Not Found.");
				}
				mi.Invoke(bllInstance, new object[] { settingManager.CaseParams });
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
				Assembly a = Assembly.LoadFrom(Path.Combine(StartUp.ModulesFileName, AppSettingManager.Instance.BusinessDllName));
				types = a.GetTypes();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			caseType = Array.Find(types, t => t.Name.Equals(className, StringComparison.CurrentCultureIgnoreCase));
			return caseType != null;
		}
	}
}