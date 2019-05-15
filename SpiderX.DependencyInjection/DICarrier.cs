using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SpiderX.DependencyInjection
{
    public sealed class DICarrier
    {
        public DICarrier() : this(new ServiceCollection())
        {
        }

        public DICarrier(IServiceCollection services)
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