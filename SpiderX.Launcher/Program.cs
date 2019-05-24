using System;
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
			bool existsCommandLine = args != null && args.Length > 1;//The length must be larger than 1.
			string settingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "AppSettings");
			var hostConf = new ConfigurationBuilder()
				.SetBasePath(settingFilePath)
				.AddJsonFile("hostsettings.json", optional: false, reloadOnChange: true)
				.Build();
			var host = new HostBuilder()
				.ConfigureHostConfiguration(c => c.AddConfiguration(hostConf))
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
				.ConfigureServices((hostContext, services) =>
				{
					bool runCasesConcurrently = args.Length > 2 && hostContext.Configuration.GetValue<bool>("RunCasesConcurrently");
					if (existsCommandLine)
					{
						string nameSpace = GetNameSpaceOfServices(hostContext.Configuration, args[0]);
						if (!runCasesConcurrently)
						{
							for (byte i = 1; i < args.Length; i++)
							{
								if (CaseOption.CheckSkipStringArg(args[i]))
								{
									continue;
								}
								services.AddHostedService<SingleBllCaseService>()
								.Configure<CaseOption>(opt =>
								{
									opt.NameSpace = nameSpace;
									CaseOption.InitOption(opt, args[i]);
								});
							}
						}
						else
						{
						}
					}
					else
					{
						services.AddHostedService<SingleBllCaseService>();
					}
				})
				.ConfigureLogging((hostContext, logConf) =>
				{
					logConf.AddConsole();
				})
				.UseConsoleLifetime()
				.Build();
			using (host)
			{
				await host.StartAsync();
				await host.StopAsync();
				bool autoClose = hostConf.GetValue<bool>("AutoClose");
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
	}
}