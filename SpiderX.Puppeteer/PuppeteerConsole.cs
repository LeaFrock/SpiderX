using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace SpiderX.Puppeteer
{
	public static class PuppeteerConsole
	{
		private readonly static BrowserFetcher _browserFetcher = new BrowserFetcher(new BrowserFetcherOptions()
		{
			Host = Environment.GetEnvironmentVariable("PUPPETEER_CHROMIUM_DOWNLOADHOST")
		});

		private readonly static List<Browser> _browsers = new List<Browser>(2);

		public static IReadOnlyList<Browser> Browsers => _browsers;

		public static async Task<Browser> LauncherBrowser(bool useHeadless, int revision = 669486)
		{
			string exePath = Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH");
			if (!File.Exists(exePath))
			{
				await _browserFetcher.DownloadAsync(revision);
			}
			var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(new LaunchOptions
			{
				Headless = useHeadless,
				ExecutablePath = exePath
			});
			_browsers.Add(browser);
			return browser;
		}

		public static void CloseAllBrowsers()
		{
			foreach (var b in _browsers)
			{
				b?.Dispose();
			}
			_browsers.Clear();
		}

		private static async Task<bool> TryDownloadChromeAsync(int revision = 0, string exePath = null)
		{
			if (revision < 1)
			{
				string revisionStr = Environment.GetEnvironmentVariable("PUPPETEER_CHROMIUM_REVISION");
				if (!int.TryParse(revisionStr, out revision))
				{
					revision = BrowserFetcher.DefaultRevision;
				}
			}
			if (File.Exists(exePath))
			{
				return true;
			}
			if (string.IsNullOrEmpty(exePath))
			{
				exePath = Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH");
				await _browserFetcher.DownloadAsync(revision);
			}
			else
			{
				await _browserFetcher.DownloadAsync(revision);
			}
			return false;
		}
	}
}