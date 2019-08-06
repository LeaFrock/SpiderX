using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace SpiderX.Puppeteer
{
	public static class PuppeteerConsole
	{
		private readonly static object _downloadSyncRoot = new object();

		private volatile static bool existsChrome;

		private readonly static List<Browser> _browsers = new List<Browser>(2);

		public static IReadOnlyList<Browser> Browsers => _browsers;

		public static async Task<Browser> LauncherBrowser(bool useHeadless, string exePath = null, int revision = 0)
		{
			if (!existsChrome)
			{
				lock (_downloadSyncRoot)
				{
					if (!existsChrome)
					{
						DownloadAsync(exePath, revision).GetAwaiter().GetResult();
						existsChrome = true;
					}
				}
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

		private static async Task DownloadAsync(string exePath, int revision)
		{
			string path = exePath ?? Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH");
			if (!File.Exists(path))
			{
				var fetchOpt = new BrowserFetcherOptions()
				{
					Host = Environment.GetEnvironmentVariable("PUPPETEER_CHROMIUM_DOWNLOADHOST"),
					Path = path
				};
				var fetcher = new BrowserFetcher(fetchOpt);
				if (revision < 1)
				{
					string revisionStr = Environment.GetEnvironmentVariable("PUPPETEER_CHROMIUM_REVISION");
					if (!int.TryParse(revisionStr, out revision) || revision < 1)
					{
						revision = BrowserFetcher.DefaultRevision;
					}
				}
				await fetcher.DownloadAsync(revision).ConfigureAwait(false);
			}
		}
	}
}