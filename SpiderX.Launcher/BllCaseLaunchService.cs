using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpiderX.BusinessBase;

namespace SpiderX.Launcher
{
	internal class BllCaseLaunchService : IHostedService
	{
		private readonly ILogger _logger;
		private readonly IConfiguration _configuration;
		private readonly IApplicationLifetime _applicationLifetime;

		private readonly List<CaseSetting> _caseSettings;

		public BllCaseLaunchService(IApplicationLifetime applicationLifetime, ILogger<BllCaseLaunchService> logger, IConfiguration configuration, IOptions<List<CaseSetting>> caseSettings)
		{
			_caseSettings = caseSettings.Value ?? throw new ArgumentNullException(nameof(caseSettings));

			_logger = logger;
			_configuration = configuration;
			_applicationLifetime = applicationLifetime;
			_applicationLifetime.ApplicationStarted.Register(OnStarted);
			_applicationLifetime.ApplicationStopping.Register(OnStopping);
			_applicationLifetime.ApplicationStopped.Register(OnStopped);
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			bool runCasesConcurrently = _configuration.GetValue<bool>("RunCasesConcurrently");
			if (runCasesConcurrently)
			{
				Task[] caseTask = new Task[_caseSettings.Count];
				for (int i = 0; i < _caseSettings.Count; i++)
				{
					caseTask[i] = LaunchBllCase(_caseSettings[i]);
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
				foreach (var setting in _caseSettings)
				{
					await LaunchBllCase(setting);
				}
			}
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			_applicationLifetime.StopApplication();
			await Task.CompletedTask;
		}

		private async Task LaunchBllCase(CaseSetting setting)
		{
			if (!TryGetCaseType(setting, out var caseType))
			{
				return;
			}
			BllBase bllInstance;
			try
			{
				bllInstance = (BllBase)Activator.CreateInstance(caseType, _logger, setting.Params, setting.Version);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "CreateBllInstance_Failed.");
				return;
			}
			await bllInstance.RunAsync();
		}

		private bool TryGetCaseType(CaseSetting setting, out Type caseType)
		{
			string fullTypeName = setting.FullTypeName;
			try
			{
				Assembly a = Assembly.Load(setting.NameSpace);
				caseType = a.GetType(fullTypeName, true, true);
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, ex.Message);
				caseType = null;
				return false;
			}
			if (caseType.IsAbstract || caseType.IsNotPublic || !caseType.IsSubclassOf(typeof(BllBase)))
			{
				_logger?.LogError("Invalid Type:" + fullTypeName);
				return false;
			}
			return true;
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