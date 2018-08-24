using System;
using System.Reflection;

namespace SpiderX.Launcher
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var setting = AppSettingManager.Instance;
			Console.ReadKey();
		}

		private static Type LoadBll(string className)
		{
			try
			{
				Assembly a = Assembly.Load("SpiderX.Business.dll");
				var types = a.GetTypes();
				return Array.Find(types, t => t.GetType().Name.Equals(className, StringComparison.CurrentCultureIgnoreCase));
			}
			catch (ReflectionTypeLoadException ex)
			{
				throw ex;
			}
		}
	}
}