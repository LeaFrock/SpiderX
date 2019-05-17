using Microsoft.Extensions.DependencyInjection;

namespace SpiderX.DependencyInjection
{
	public sealed class SpiderServiceContainer
	{
		public SpiderServiceContainer() : this(new ServiceCollection())
		{
		}

		public SpiderServiceContainer(IServiceCollection services)
		{
			Services = services;
		}

		public IServiceCollection Services { get; }
	}
}