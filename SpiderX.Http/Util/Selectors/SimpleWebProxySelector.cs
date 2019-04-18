using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using SpiderX.Proxy;

namespace SpiderX.Http.Util.Selectors
{
    public sealed class SimpleWebProxySelector : IWebProxySelector
    {
        public SimpleWebProxySelector(Uri uri, IProxyUriLoader uriLoader, Func<IWebProxy, HttpClient> clientFactory, Predicate<string> rspValidator = null)
        {
            TargetUri = uri ?? throw new ArgumentNullException(nameof(TargetUri));
            _proxyUriLoader = uriLoader ?? throw new ArgumentNullException(nameof(_proxyUriLoader));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(_clientFactory));
            _responseValidator = rspValidator;
        }

        private readonly Predicate<string> _responseValidator;
        private readonly Func<IWebProxy, HttpClient> _clientFactory;
        private readonly IProxyUriLoader _proxyUriLoader;

        private readonly ConcurrentQueue<WebProxy> _normalQueue = new ConcurrentQueue<WebProxy>();
        private readonly ConcurrentQueue<WebProxy> _advancedQueue = new ConcurrentQueue<WebProxy>();

        public Uri TargetUri { get; }

        public int AdvancedProxiesCacheCapacity { get; set; } = 100;

        public void OnAdvancedProxyFail(WebProxy proxy)
        {
            throw new NotImplementedException();
        }

        public void OnAdvancedProxySuccess(WebProxy proxy)
        {
            throw new NotImplementedException();
        }

        public void OnNormalProxyFail(WebProxy proxy)
        {
            throw new NotImplementedException();
        }

        public void OnNormalProxySuccess(WebProxy proxy)
        {
            throw new NotImplementedException();
        }

        public WebProxy SelectNextProxy()
        {
            throw new NotImplementedException();
        }

        public bool TryPreferAdvancedProxy(out WebProxy proxy)
        {
            throw new NotImplementedException();
        }

        private void LoadProxies(int maxCount = 0)
        {
            var uris = _proxyUriLoader.Load(maxCount);
            foreach (var uri in uris)
            {
                _normalQueue.Enqueue(new WebProxy(uri));
            }
        }
    }
}