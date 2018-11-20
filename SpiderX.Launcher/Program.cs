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
			AppSettingManager settingManager;
			string caseName;
			string[] caseParams;
			if (args.IsNullOrEmpty())
			{
				settingManager = AppSettingManager.Instance;
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
				settingManager = AppSettingManager.CreateInstance(caseName, caseParams);
				caseName = settingManager.CaseName;
			}
			//StartUp
			StartUp.Run();
			//Get TargetType
			if (!TryGetType(caseName, out Type bllType))
			{
				return;
			}
			if (!bllType.IsClass || bllType.IsAbstract || bllType.IsNotPublic)
			{
				throw new TypeAccessException("Invalid Type: " + settingManager.CaseName);
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
			if (caseParams.IsNullOrEmpty())
			{
				MethodInfo mi = bllType.GetMethod(methodName, Type.EmptyTypes);
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
				Console.WriteLine("Program Run Over.");
				Console.ReadKey();
			}
		}

		private static bool TryGetType(string className, out Type caseType)
		{
			var settingManager = AppSettingManager.Instance;
			string loadPath = Path.Combine(Directory.GetCurrentDirectory(), StartUp.ModulesDirectoryName, className, settingManager.BusinessModuleName + ".dll");
			Assembly a;
			try
			{
				a = Assembly.LoadFrom(loadPath);
			}
			catch
			{
				caseType = null;
				return false;
			}
			string fullClassName = settingManager.BusinessModuleName + '.' + className;
			caseType = a.GetType(fullClassName, false, true);
			return caseType != null;
		}
	}
}