using SpiderX.Proxy;
using SpiderX.Tools;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SpiderX.Http.Util
{
    public sealed partial class DefaultSpiderProxyUriSelector : SpiderProxyUriSelectorBase
    {
        public override bool HasNextProxy => !_internalQueue.IsEmpty;

        public override int StatusCode => _initTimes;

        public override void Init(IEnumerable<Uri> proxies)
        {
            if (Interlocked.CompareExchange(ref _initTimes, 1, 0) != 0)
            {
                return;
            }
            InsertFreshProxies(proxies);
        }

        public override void InsertFreshProxies(IEnumerable<Uri> proxies)
        {
            foreach (var proxy in proxies)
            {
                _internalQueue.Enqueue(proxy);
            }
        }

        public override Uri SelectNextProxyUri()
        {
            if (ProxyUriInterval <= TimeSpan.Zero)
            {
                while (true)
                {
                    if (_internalQueue.TryDequeue(out Uri uri))
                    {
                        _internalQueue.Enqueue(uri);
                        return uri;
                    }
                    Thread.Sleep(CommonTool.RandomEvent.Next(3000, 7000));
                }
            }
            else
            {
                while (true)
                {
                    if (_internalQueue.TryDequeue(out Uri uri))
                    {
                        _internalQueue.Enqueue(uri);
                        DateTime dt = _latestUseTimeRecords.GetOrAdd(uri, DateTime.MinValue);
                        DateTime now = DateTime.Now;
                        if (now > dt.Add(ProxyUriInterval))
                        {
                            _latestUseTimeRecords.TryUpdate(uri, now, now);
                            return uri;
                        }
                    }
                    Thread.Sleep(CommonTool.RandomEvent.Next(3000, 7000));
                }
            }
        }

        public override Uri SelectGoodProxyUri()
        {
            if (ProxyUriInterval <= TimeSpan.Zero)
            {
                while (true)
                {
                    if (_internalQueue.TryDequeue(out Uri uri))
                    {
                        _internalQueue.Enqueue(uri);
                        return uri;
                    }
                    Thread.Sleep(CommonTool.RandomEvent.Next(3000, 7000));
                }
            }
            else
            {
                while (true)
                {
                    if (_internalQueue.TryDequeue(out Uri uri))
                    {
                        _internalQueue.Enqueue(uri);
                        DateTime dt = _latestUseTimeRecords.GetOrAdd(uri, DateTime.MinValue);
                        DateTime now = DateTime.Now;
                        if (now > dt.Add(ProxyUriInterval))
                        {
                            _latestUseTimeRecords.TryUpdate(uri, now, now);
                            return uri;
                        }
                    }
                    Thread.Sleep(CommonTool.RandomEvent.Next(3000, 7000));
                }
            }
        }
    }
}