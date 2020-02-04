using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace SpiderX.Puppeteer
{
    public static class PuppeteerConsole
    {
        private readonly static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        private readonly static ConcurrentBag<Browser> _browsers = new ConcurrentBag<Browser>();

        public static IReadOnlyCollection<Browser> Browsers => _browsers;

        public static DirectoryInfo DefaultDirectory => new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SpiderX", "local-chromium"));

        public static async Task<Browser> LauncherBrowser(bool useHeadless, DirectoryInfo directory = null, int revision = 0)
        {
            var dir = directory ?? DefaultDirectory;
            if (!TryGetChromeExePath(dir, out var exeFile))
            {
                exeFile = await DownloadAsync(dir, revision);
                if (exeFile is null)
                {
                    throw new IOException("Download of chrome.exe failed.");
                }
            }
            var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = useHeadless,
                ExecutablePath = exeFile.FullName
            });
            _browsers.Add(browser);
            return browser;
        }

        public static void CloseAllBrowsers()
        {
            if (_browsers.IsEmpty)
            {
                return;
            }
            var bs = _browsers.ToArray();
            _browsers.Clear();
            foreach (var b in bs)
            {
                b?.Dispose();
            }
        }

        private static async Task<FileInfo> DownloadAsync(DirectoryInfo directory, int revision)
        {
            await _semaphoreSlim.WaitAsync();
            if (!directory.Exists)
            {
                directory.Create();
            }
            var fetchOpt = new BrowserFetcherOptions()
            {
                Host = Environment.GetEnvironmentVariable("PUPPETEER_CHROMIUM_DOWNLOADHOST"),
                Path = directory.FullName
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
            var info = await fetcher.DownloadAsync(revision).ConfigureAwait(false);
            _semaphoreSlim.Release();
            if (!string.IsNullOrWhiteSpace(info.ExecutablePath))
            {
                return new FileInfo(info.ExecutablePath);
            }
            TryGetChromeExePath(directory, out var exeFile);
            return exeFile;
        }

        private static bool TryGetChromeExePath(DirectoryInfo directory, out FileInfo exeFile)
        {
            exeFile = null;
            if (!directory.Exists)
            {
                return false;
            }
            var files = directory.GetFiles("chrome.exe", SearchOption.AllDirectories);
            if (files is null || files.Length < 1)
            {
                return false;
            }
            exeFile = files[0];
            return true;
        }
    }
}