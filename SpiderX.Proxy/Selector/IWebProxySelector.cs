using System.Net;

namespace SpiderX.Proxy
{
    public interface IWebProxySelector
    {
        WebProxy SelectNextProxy();

        bool TryPreferAdvancedProxy(out WebProxy proxy);

        void OnNormalProxyFail(WebProxy proxy);

        void OnNormalProxySuccess(WebProxy proxy);

        void OnAdvancedProxyFail(WebProxy proxy);

        void OnAdvancedProxySuccess(WebProxy proxy);
    }
}