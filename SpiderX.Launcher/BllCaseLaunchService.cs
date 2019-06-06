using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
			_caseSettings = caseSettings.Value;
			if (_caseSettings == null || _caseSettings.Count < 1)
			{
				throw new ArgumentException("Invalid CaseSettings.");
			}
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
			string fullTypeName = setting.FullTypeName;
			Type caseType;
			try
			{
				Assembly a = Assembly.Load(setting.NameSpace);
				caseType = a.GetType(fullTypeName, true, true);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				return;
			}
			if (caseType.IsAbstract || caseType.IsNotPublic || !caseType.IsSubclassOf(typeof(BllBase)))
			{
				_logger.LogError("Invalid Type:" + fullTypeName);
				return;
			}
			//Create Instance
			BllBase bllInstance;
			try
			{
				bllInstance = (BllBase)Activator.CreateInstance(caseType, false);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.StackTrace);
				return;
			}
			//Invoke Method
			var runParams = setting.Params;
			if (runParams == null || runParams.Length < 1)
			{
				await Task.Run(bllInstance.Run);
			}
			else
			{
				await Task.Run(() => bllInstance.Run(runParams));
			}
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