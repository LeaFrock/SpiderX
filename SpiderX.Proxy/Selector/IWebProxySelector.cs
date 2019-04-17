using System.Net;

namespace SpiderX.Proxy
{
    public interface IWebProxySelector
    {
        WebProxy SelectNextProxy();

        WebProxy SelectGoodProxy();
    }
}