using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SpiderX.Launcher
{
	internal class BllCaseLaunchService : IHostedService
	{
		private readonly ILogger _logger;
		private readonly IConfiguration _configuration;
		private readonly IApplicationLifetime _applicationLifetime;

		private readonly List<CaseOption> _caseSettings;

		private readonly static Random random = new Random();

		public BllCaseLaunchService(IApplicationLifetime applicationLifetime, ILogger<BllCaseLaunchService> logger, IConfiguration configuration, IOptions<List<CaseOption>> caseSettings)
		{
			_logger = logger;
			_configuration = configuration;
			_applicationLifetime = applicationLifetime;
			_caseSettings = caseSettings.Value;
			_applicationLifetime.ApplicationStarted.Register(OnStarted);
			_applicationLifetime.ApplicationStopping.Register(OnStopping);
			_applicationLifetime.ApplicationStopped.Register(OnStopped);
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogWarning(random.Next(0, 10).ToString());
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_applicationLifetime.StopApplication();
			return Task.CompletedTask;
		}

		private void OnStarted()
		{
			_logger.LogInformation("Started...");
		}

		private void OnStopping()
		{
			_logger.LogInformation("Stopping...");
		}

		private void OnStopped()
		{
			_logger.LogInformation("Stopped...");
		}
	}
}