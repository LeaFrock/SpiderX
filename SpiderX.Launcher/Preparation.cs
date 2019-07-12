using System;
using System.IO;
using System.Text;

namespace SpiderX.Launcher
{
	internal static class Preparation
	{
		public static void JustDoIt()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			VerifyEnvironmentVariables();
		}

		private static void VerifyEnvironmentVariables()
		{
			string puppeteerExecutablePath = Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH");
			if (string.IsNullOrEmpty(puppeteerExecutablePath))
			{
				string path = Path.Combine(Directory.GetCurrentDirectory(), ".local-chromium", "chrome.exe");
				Environment.SetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH", path);
			}
			else
			{
				Environment.SetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH", puppeteerExecutablePath.Replace("*", Directory.GetCurrentDirectory()));
			}
			//puppeteerExecutablePath = Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH");
		}
	}
}