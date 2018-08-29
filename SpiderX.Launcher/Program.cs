using System;
using System.Reflection;
using SpiderX.Business;

namespace SpiderX.Launcher
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var setting = AppSettingManager.Instance;
			if (string.IsNullOrWhiteSpace(setting.CaseName))
			{
				throw new ArgumentNullException("CaseName is Null or Empty.");
			}
			if (!TryGetCaseType(setting.CaseName, out Type bllType))
			{
				throw new TypeLoadException(setting.CaseName + " Not Found.");
			}

			Console.ReadKey();
		}

		private static bool TryGetCaseType(string className, out Type caseType)
		{
			try
			{
				Assembly a = typeof(BllBase).Assembly;
				var types = a.GetTypes();
				caseType = Array.Find(types, t => t.Name.Equals(className, StringComparison.CurrentCultureIgnoreCase));
				return true;
			}
			catch //(Exception ex)
			{
				//throw ex;
				caseType = null;
				return false;
			}
		}
	}
}