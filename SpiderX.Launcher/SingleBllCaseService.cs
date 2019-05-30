using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SpiderX.Launcher
{
	internal class SingleBllCaseService : IHostedService
	{
		private readonly ILogger _logger;
		private readonly IApplicationLifetime _applicationLifetime;
		private readonly CaseOption _caseSetting;

		public SingleBllCaseService(IApplicationLifetime applicationLifetime, ILogger<SingleBllCaseService> logger, IOptions<CaseOption> caseSetting)
		{
			_caseSetting = caseSetting.Value ?? throw new ArgumentNullException();
			_logger = logger;
			_applicationLifetime = applicationLifetime;
			_applicationLifetime.ApplicationStarted.Register(OnStarted);
			_applicationLifetime.ApplicationStopping.Register(OnStopping);
			_applicationLifetime.ApplicationStopped.Register(OnStopped);
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation(_caseSetting.FullTypeName);
			return Task.Delay(5000);
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_applicationLifetime.StopApplication();
			return Task.CompletedTask;
		}

		private void OnStarted()
		{
			_logger.LogDebug("Started...");
		}

		private void OnStopping()
		{
			_logger.LogDebug("Stopping...");
		}

		private void OnStopped()
		{
			_logger.LogDebug("Stopped...");
		}
	}
}