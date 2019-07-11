using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace SpiderX.Puppeteer
{
	public static class PuppeteerConsole
	{
		private readonly static BrowserFetcher _browserFetcher = new BrowserFetcher();

		private volatile static int _downloadTimes;

		private volatile static int _browserCount;

		public static int BrowserCount => _browserCount;

		public static async Task<Browser> LauncherBrowser(int revision = 669486)
		{
			string filePath = Path.Combine(Directory.GetCurrentDirectory(), ".local-chromium");
			//File.Exists(filePath)
			await _browserFetcher.DownloadAsync(revision);
			var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(new LaunchOptions
			{
				Headless = true
			});
			Interlocked.Increment(ref _browserCount);
			return null;
			//return browser;
		}
	}
}