using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpiderX.BusinessBase;
using SpiderX.DataClient;

namespace SpiderX.Launcher
{
	internal class Program
	{
		private async static Task Main(string[] args)
		{
			Preparation.JustDoIt();
			string settingFilePath = Directory.GetCurrentDirectory();
			var hostConfig = new ConfigurationBuilder()
				.SetBasePath(settingFilePath)
				.AddJsonFile("hostsettings.json", optional: false, reloadOnChange: true)
				.Build();
			bool existsCommandLine = ExistsValidCommandLine(args, out int validLength);
			var hostBuilder = new HostBuilder()
				.ConfigureHostConfiguration(hostConf => hostConf.AddConfiguration(hostConfig))
				.ConfigureAppConfiguration((hostContext, appConf) =>
				{
					if (existsCommandLine)
					{
						appConf.AddCommandLine(args);
					}
					else
					{
						appConf.SetBasePath(settingFilePath);
						appConf.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
					}
				})
				.ConfigureLogging((hostContext, logConf) =>
				{
					logConf.AddConsole();
				});
			hostBuilder.ConfigureServices((hostContext, services) =>
			{
				services.AddSingleton<DbConfigManager>();
				services.AddHostedService<BllCaseLaunchService>()
				.Configure<List<IBllCaseBuilder>>(opts =>
				{
					var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();
					if (existsCommandLine)
					{
						string nmsp = GetNameSpaceOfServices(hostConfig, args[0]);
						for (byte i = 1; i < validLength; i++)
						{
							string cmd = args[i];
							var opt = BllCaseBuilderHelper.FromCommandLine(cmd, nmsp, loggerFactory);
							opts.Add(opt);
						}
					}
					else
					{
						var hostConf = hostContext.Configuration;
						var casesConf = hostConf.GetSection("CaseSettings").GetChildren();
						foreach (var cs in casesConf)
						{
							bool enabled = cs.GetValue<bool>("Enabled");
							if (enabled)
							{
								var opt = BllCaseBuilderHelper.FromConfiguration(cs, loggerFactory);
								opts.Add(opt);
							}
						}
					}
				});
			});
			hostBuilder.UseConsoleLifetime();
			using var host = hostBuilder.Build();
			DbConfigManager.SetDefault(host.Services);
			await host.StartAsync();
			bool autoClose = hostConfig.GetValue<bool>("AutoClose");
			if (!autoClose)
			{
				Console.ReadKey();
			}
			await host.StopAsync();
		}

		private static string GetNameSpaceOfServices(IConfiguration hostConf, string arg)
		{
			var nameSpaceAbbrs = hostConf.GetSection("BllNameSpaces").GetChildren();
			foreach (var section in nameSpaceAbbrs)
			{
				string abbr = section.GetValue<string>("Abbr");
				if (arg.Equals(abbr, StringComparison.CurrentCultureIgnoreCase))
				{
					return section.GetValue<string>("Name");
				}
			}
			return arg;
		}

		private static bool ExistsValidCommandLine(string[] args, out int validLength)
		{
			if (args == null || args.Length < 2)//The length must be larger than 1.
			{
				validLength = 0;
				return false;
			}
			int validCount = 0;
			for (byte i = 1; i < args.Length; i++)
			{
				if (!CheckSkipStringArg(args[i]))
				{
					validCount++;
					string temp = args[validCount];
					args[validCount] = args[i];
					args[i] = temp;
				}
			}
			if (validCount < 1)
			{
				validLength = 0;
				return false;
			}
			validLength = validCount + 1;
			return true;
		}

		public static bool CheckSkipStringArg(string arg) => arg.StartsWith('/');
	}
}