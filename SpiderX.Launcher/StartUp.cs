using System;
using System.IO;
using System.Text;

namespace SpiderX.Launcher
{
	public sealed class StartUp
	{
		public const string ModulesDirectoryName = "BusinessModules";

		public static string ModulesDirectoryPath { get; private set; }

		public void Run()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			CheckModulesFile();
		}

		private void CheckModulesFile()
		{
			ModulesDirectoryPath = Path.Combine(Environment.CurrentDirectory, ModulesDirectoryName);
			if (!Directory.Exists(ModulesDirectoryPath))
			{
				Directory.CreateDirectory(ModulesDirectoryPath);
			}
			AppSettingManager.Instance.CopyModuleTo(ModulesDirectoryPath);
		}
	}
}