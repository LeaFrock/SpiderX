using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SpiderX.ProxyFilter
{
	internal class Program
	{
		private readonly static Dictionary<RunModeEnum, Func<IConfiguration, IProxyReceiver>> _receiverFactories = new Dictionary<RunModeEnum, Func<IConfiguration, IProxyReceiver>>()
		{
			{ RunModeEnum.Redis, (conf) => new RedisProxyReceiver(conf["RedisConfig:ConnectionString"], conf["RedisConfig:ChannelName"]) }
		};

		private static async Task Main(string[] args)
		{
			string settingFilePath = Directory.GetCurrentDirectory();
			var appConfig = new ConfigurationBuilder()
				.SetBasePath(settingFilePath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();
			RunModeEnum runMode = GetRunMode(args, appConfig);
			if (!_receiverFactories.TryGetValue(runMode, out var factory))
			{
				ShowConsoleMsg("Error", "Invalid Mode.");
			}
			var receiver = factory.Invoke(appConfig);
			string receiverName = receiver.GetType().Name;
			receiver.OnProxyReceived += (sender, eArgs) =>
			{
				ShowConsoleMsg(receiverName, eArgs.Proxies.Length.ToString());
			};
			await receiver.InitializeAsync();
			while (true)
			{
				ShowConsoleMsg("Info", "Press K for Exiting.");
				var k = Console.ReadKey();
				if (k.Key == ConsoleKey.K)
				{
					break;
				}
			}
			receiver.Disable();
		}

		private static RunModeEnum GetRunMode(string[] args, IConfigurationRoot appConfig)
		{
			if (args != null && args.Length == 1 && !string.IsNullOrEmpty(args[0]) && Enum.TryParse(args[0], true, out RunModeEnum mode))
			{
				ShowConsoleMsg("Info", "Read the value of 'Mode' from init-params successfully.");
				return mode;
			}
			ShowConsoleMsg("Info", "Cannot read the value of 'Mode' from init-params. Try to read 'appsettings.json' instead.");
			byte modeNum = appConfig.GetValue<byte>("Mode");
			if (Enum.IsDefined(typeof(RunModeEnum), modeNum))
			{
				return (RunModeEnum)modeNum;
			}
			ShowConsoleMsg("Info", "Cannot read the value of 'Mode' from json-config. Please input it manually: ");
			ConsoleWriteModeEnum();
			string modeStr;
			while (true)
			{
				if (!string.IsNullOrEmpty(modeStr = Console.ReadLine()) && Enum.TryParse(modeStr, true, out mode) && Enum.IsDefined(typeof(RunModeEnum), mode))
				{
					return mode;
				}
				ShowConsoleMsg("Error", "Input the wrong value of 'Mode'. Please try again.");
			}
		}

		private static void ConsoleWriteModeEnum()
		{
			var modeValues = Enum.GetValues(typeof(RunModeEnum));
			Console.ForegroundColor = ConsoleColor.Yellow;
			foreach (var v in modeValues)
			{
				Console.WriteLine($"	{((byte)v).ToString("d3")} {v.ToString()}");
			}
			Console.ResetColor();
		}

		private static void ShowConsoleMsg(string title, string content)
		{
			StringBuilder sb = new StringBuilder(4);
			sb.Append(DateTime.Now.ToString("[MM/dd-hh:mm:ss]["));
			sb.Append(title);
			sb.Append("] ");
			sb.Append(content);
			string msg = sb.ToString();
			Console.WriteLine(msg);
		}
	}
}