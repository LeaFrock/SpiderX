using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SpiderX.DependencyInjection
{
	public sealed class DIContainer
	{
		public DIContainer() : this(new ServiceCollection())
		{
		}

		public DIContainer(IServiceCollection services)
		{
			Services = services;
		}

		public IServiceCollection Services { get; }

		public void ConfigureServices()
		{
			Services.AddLogging();
			Services.AddHttpClient<HttpClient>();
		}
	}
}