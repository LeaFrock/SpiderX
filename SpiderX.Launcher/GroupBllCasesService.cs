using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SpiderX.Launcher
{
	internal class GroupBllCasesService : IHostedService
	{
		private readonly ILogger _logger;
		private readonly IConfiguration _configuration;
		private readonly IApplicationLifetime _applicationLifetime;

		private readonly IEnumerable<CaseOption> _caseSettings;

		private readonly static Random random = new Random();

		public GroupBllCasesService(IApplicationLifetime applicationLifetime, ILogger<SingleBllCaseService> logger, IConfiguration configuration, IEnumerable<CaseOption> caseSettings)
		{
			_logger = logger;
			_configuration = configuration;
			_applicationLifetime = applicationLifetime;
			_caseSettings = caseSettings;
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