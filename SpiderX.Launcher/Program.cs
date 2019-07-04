using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpiderX.DataClient;

namespace SpiderX.Launcher
{
	internal class Program
	{
		private async static Task Main(string[] args)
		{
			Preparation.JustDoIt();
			string settingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "AppSettings");
			var hostConfig = new ConfigurationBuilder()
				.SetBasePath(settingFilePath)
				.AddJsonFile("hostsettings.json", optional: false, reloadOnChange: true)
				.Build();
			bool existsCommandLine = ExistsValidCommandLine(args, out string[] validArgs);
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
				.Configure<List<CaseSetting>>(opts =>
				{
					if (existsCommandLine)
					{
						string nmsp = GetNameSpaceOfServices(hostConfig, args[0]);
						for (byte i = 1; i < args.Length; i++)
						{
							string cmd = args[i];
							if (!CaseSetting.CheckSkipStringArg(cmd))
							{
								var opt = CaseSetting.FromCommandLine(cmd, nmsp);
								opts.Add(opt);
							}
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
								var opt = CaseSetting.FromConfiguration(cs);
								opts.Add(opt);
							}
						}
					}
				});
			});
			hostBuilder.UseConsoleLifetime();
			var host = hostBuilder.Build();
			using (host)
			{
				DbConfigManager.SetDefault(host.Services);
				await host.StartAsync();
				await host.StopAsync();
				bool autoClose = hostConfig.GetValue<bool>("AutoClose");
				if (!autoClose)
				{
					Console.ReadKey();
				}
			}
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

		private static bool ExistsValidCommandLine(string[] args, out string[] validArgs)
		{
			if (args == null || args.Length < 2)//The length must be larger than 1.
			{
				validArgs = null;
				return false;
			}
			int validCount = 0;
			for (byte i = 1; i < args.Length; i++)
			{
				if (!CaseSetting.CheckSkipStringArg(args[i]))
				{
					validCount++;
					args[validCount] = args[i];
				}
			}
			if (validCount < 1)
			{
				validArgs = null;
				return false;
			}
			validArgs = new string[validCount + 1];
			Array.Copy(args, validArgs, validCount + 1);
			return true;
		}
	}
}