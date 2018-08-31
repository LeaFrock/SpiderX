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
			var settingManager = AppSettingManager.Instance;
			if (!Directory.Exists(ModulesDirectoryPath))
			{
				Directory.CreateDirectory(ModulesDirectoryPath);
				settingManager.CopyModuleTo(ModulesDirectoryPath);
			}
			else
			{
				switch (settingManager.BusinessModuleCopyMode)
				{
					case BusinessModuleCopyModeEnum.AlwaysCopy:
						settingManager.CopyModuleTo(ModulesDirectoryPath);
						break;

					case BusinessModuleCopyModeEnum.CopyOnce:
						if (File.Exists(ModulesDirectoryPath))
						{
							break;
						}
						settingManager.CopyModuleTo(ModulesDirectoryPath);
						break;

					default:
						break;
				}
			}
		}
	}
}