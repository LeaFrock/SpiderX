using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpiderX.Proxy
{
	public interface IWebProxyValidator
	{
		WebProxyValidatorConfig Config { get; }

		Func<IWebProxy, HttpClient> ClientFactory { get; }

		Predicate<string> ResponseTextValidator { get; }

		Task<bool> VerifyProxyAsync<T>(T proxy, Uri targetUri) where T : IWebProxy;
	}
}