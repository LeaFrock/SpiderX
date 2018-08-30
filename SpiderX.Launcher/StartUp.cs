using System;
using System.IO;
using System.Text;

namespace SpiderX.Launcher
{
	public sealed class StartUp
	{
		public const string ModulesFileName = "Modules";

		public static string ModulesFilePath { get; private set; }

		public void Run()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			CheckModulesFile();
		}

		private void CheckModulesFile()
		{
			string path = Path.Combine(Environment.CurrentDirectory, ModulesFileName);
			Directory.CreateDirectory(path);
			ModulesFilePath = path;
		}
	}
}