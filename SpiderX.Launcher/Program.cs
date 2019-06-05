using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SpiderX.Launcher
{
	internal class Program
	{
		private async static Task Main(string[] args)
		{
			Preparation.JustDoIt();
			bool existsCommandLine = ExistsValidCommandLine(args, out string[] validArgs);
			string settingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "AppSettings");
			var hostConfig = new ConfigurationBuilder()
				.SetBasePath(settingFilePath)
				.AddJsonFile("hostsettings.json", optional: false, reloadOnChange: true)
				.Build();
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
				services.AddHostedService<BllCaseLaunchService>()
				.Configure<List<CaseOption>>(opts =>
				{
					if (existsCommandLine)
					{
						string nmsp = GetNameSpaceOfServices(hostConfig, args[0]);
						for (byte i = 1; i < args.Length; i++)
						{
							string cmd = args[i];
							if (!CaseOption.CheckSkipStringArg(cmd))
							{
								CaseOption opt = new CaseOption() { NameSpace = nmsp };
								opt.InitByCommandLine(cmd);
								opts.Add(opt);
							}
						}
					}
					else
					{
						var hostConf = hostContext.Configuration;
					}
				});
			});
			hostBuilder.UseConsoleLifetime();
			var host = hostBuilder.Build();
			using (host)
			{
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
				string abbr = section.GetSection("Abbr").Value;
				if (arg.Equals(abbr, StringComparison.CurrentCultureIgnoreCase))
				{
					return section.GetSection("Name").Value;
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
			List<string> tempList = new List<string>(args.Length) { args[0] };
			for (byte i = 1; i < args.Length; i++)
			{
				if (!CaseOption.CheckSkipStringArg(args[i]))
				{
					tempList.Add(args[i]);
				}
			}
			if (tempList.Count < 2)
			{
				validArgs = null;
				return false;
			}
			validArgs = tempList.ToArray();
			return true;
		}
	}
}