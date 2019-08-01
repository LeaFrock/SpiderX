using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpiderX.BusinessBase;
using SpiderX.Http;

namespace SpiderX.Launcher
{
	internal class BllCaseLaunchService : IHostedService
	{
		private readonly ILogger _logger;
		private readonly IConfiguration _configuration;
		private readonly IApplicationLifetime _applicationLifetime;

		private readonly List<IBllCaseBuilder> _caseBuilders;

		public BllCaseLaunchService(IApplicationLifetime applicationLifetime, ILogger<BllCaseLaunchService> logger, IConfiguration configuration, IOptions<List<IBllCaseBuilder>> caseBuilders)
		{
			_caseBuilders = caseBuilders.Value ?? throw new ArgumentNullException(nameof(caseBuilders));

			_logger = logger;
			_configuration = configuration;
			_applicationLifetime = applicationLifetime;
			_applicationLifetime.ApplicationStarted.Register(OnStarted);
			_applicationLifetime.ApplicationStopping.Register(OnStopping);
			_applicationLifetime.ApplicationStopped.Register(OnStopped);
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			var bllCases = new List<BllBase>(_caseBuilders.Count);
			foreach (var builder in _caseBuilders)
			{
				BllBase bllCase;
				try
				{
					bllCase = builder.Build();
				}
				catch (BllCaseBuildException ex)
				{
					_logger.LogError(ex, ex.Message);
					continue;
				}
				bllCases.Add(bllCase);
			}
			bool runCasesConcurrently = _configuration.GetValue<bool>("RunCasesConcurrently");
			if (runCasesConcurrently)
			{
				Task[] caseTask = new Task[bllCases.Count];
				for (int i = 0; i < bllCases.Count; i++)
				{
					caseTask[i] = bllCases[i].RunAsync();
				}
				try
				{
					Task.WaitAll(caseTask);
				}
				catch (Exception ex)
				{
					_logger.LogTrace(ex, ex.Message);
				}
			}
			else
			{
				foreach (var bllCase in bllCases)
				{
					await bllCase.RunAsync();
				}
			}
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			HttpRequestCounter.DisposeStaticTimer();
			_applicationLifetime.StopApplication();
			await Task.CompletedTask;
		}

		private void OnStarted()
		{
			_logger.LogInformation("Started...");
		}

		private void OnStopping()
		{
			//_logger.LogInformation("Stopping...");
		}

		private void OnStopped()
		{
			_logger.LogInformation("Stopped...");
		}
	}
}