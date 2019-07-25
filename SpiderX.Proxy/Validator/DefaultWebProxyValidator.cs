using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SpiderX.Proxy
{
	public sealed class DefaultWebProxyValidator : IWebProxyValidator
	{
		public DefaultWebProxyValidator(Func<IWebProxy, HttpClient> clientFactory, Predicate<string> responseTextValidator, WebProxyValidatorConfig config = null)
		{
			_config = config;
			ClientFactory = clientFactory;
			ResponseTextValidator = responseTextValidator;
		}

		private WebProxyValidatorConfig _config;

		public WebProxyValidatorConfig Config
		{
			get
			{
				if (_config == null)
				{
					Interlocked.CompareExchange(ref _config, new WebProxyValidatorConfig(), null);
				}
				return _config;
			}
		}

		public Func<IWebProxy, HttpClient> ClientFactory { get; }

		public Predicate<string> ResponseTextValidator { get; }

		async Task<bool> IWebProxyValidator.VerifyProxyAsync<T>(T proxy, Uri targetUri)
		{
			using (HttpClient client = ClientFactory.Invoke(proxy))
			{
				string rspText = await client.GetStringAsync(targetUri);
				if (string.IsNullOrWhiteSpace(rspText))
				{
					return false;
				}
				return ResponseTextValidator?.Invoke(rspText.Trim()) != false;
			}
		}
	}
}