using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SpiderX.Proxy;

namespace SpiderX.Http.Util
{
    public sealed class DefaultWebProxySelector : IWebProxySelector
    {
        public DefaultWebProxySelector(Uri uri, ResponseValidatorBase responseValidator, IProxyUriLoader uriLoader)
        {
            _responseValidator = responseValidator;
            TargetUri = uri;
            ProxyUriLoader = uriLoader;
        }

        private readonly ResponseValidatorBase _responseValidator;

        private readonly ConcurrentQueue<WebProxy> _verifyingQueue = new ConcurrentQueue<WebProxy>();
        private readonly ConcurrentQueue<WebProxy> _availableQueue = new ConcurrentQueue<WebProxy>();
        private readonly ConcurrentQueue<WebProxy> _advancedQueue = new ConcurrentQueue<WebProxy>();

        private int _initCode;

        public bool IsInited => _initCode != 0;

        public int UseThresold { get; set; } = 100;

        public int VerifyPauseThresold { get; set; } = 300;

        public int VerifyTaskDegree { get; set; } = 100;

        public TimeSpan VerifyTaskTimeout { get; set; } = TimeSpan.FromSeconds(20);

        public TimeSpan VerifyTimeout { get; set; } = TimeSpan.FromSeconds(3);

        public Uri TargetUri { get; }

        public IProxyUriLoader ProxyUriLoader { get; set; }

        public void Initialize()
        {
            if (Interlocked.CompareExchange(ref _initCode, 1, 0) == 0)
            {
                LoadProxies();
                Task.Factory.StartNew(VerifyProxies, TaskCreationOptions.LongRunning);
            }
        }

        public WebProxy SelectNextProxy()
        {
            throw new NotImplementedException();
        }

        public WebProxy SelectGoodProxy()
        {
            throw new NotImplementedException();
        }

        private void LoadProxies()
        {
            var uris = ProxyUriLoader.Load();
            foreach (var uri in uris)
            {
                _verifyingQueue.Enqueue(new WebProxy(uri));
            }
        }

        private void VerifyProxies()
        {
            while (true)
            {
                if (_availableQueue.Count >= VerifyPauseThresold)
                {
                    Thread.Sleep(30000);
                }
                if (_verifyingQueue.IsEmpty)
                {
                    LoadProxies();
                }
                var tasks = new List<Task>(VerifyTaskDegree);
                for (int i = 0; i < VerifyTaskDegree; i++)
                {
                    if (!_verifyingQueue.TryDequeue(out var proxy))
                    {
                        break;
                    }
                    tasks.Add(VerifyProxyAsync(proxy));
                }
                try
                {
                    Task.WaitAll(tasks.ToArray(), VerifyTaskTimeout);
                }
                catch
                { }
            }
        }

        private async Task VerifyProxyAsync(WebProxy proxy)
        {
            var handler = new SocketsHttpHandler()
            {
                UseProxy = true,
                Proxy = proxy,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                ConnectTimeout = VerifyTimeout,
                Expect100ContinueTimeout = VerifyTimeout,
                ResponseDrainTimeout = VerifyTimeout
            };
            using (HttpClient client = new HttpClient(handler))
            {
                string rspText = await client.GetStringAsync(TargetUri);
                if (string.IsNullOrWhiteSpace(rspText))
                {
                    return;
                }
                if (!_responseValidator.CheckPass(rspText))
                {
                    return;
                }
                _availableQueue.Enqueue(proxy);
            }
        }
    }
}