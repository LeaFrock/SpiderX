using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SpiderX.Extensions;

namespace SpiderX.Launcher
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			//Init GlobalSetting
			string filePath = Path.Combine(Directory.GetCurrentDirectory(), "AppSettings");
			var conf = new ConfigurationBuilder()
				.SetBasePath(filePath)
				.AddJsonFile("appsettings.json", true, true)
				.Build();
			GlobalSetting.Initialize(conf);
			//Load Cases
			if (args.IsNullOrEmpty())
			{
				GlobalSetting.LoadCaseSettings(conf);
			}
			else
			{
				var caseSettings = CaseSetting.GetListByStringArgs(args);
				GlobalSetting.LoadCaseSettings(conf, caseSettings);
			}
			//StartUp
			StartUp.Run();
			//Invoke Cases
			if (!GlobalSetting.RunCasesConcurrently)
			{
				foreach (var caseSetting in GlobalSetting.CaseSettings)
				{
					caseSetting.InvokeCase();
				}
			}
			else
			{
				Parallel.ForEach(GlobalSetting.CaseSettings,
					new ParallelOptions() { MaxDegreeOfParallelism = 100 },
					s => s.InvokeCase());
			}
			//End
			if (!GlobalSetting.AutoClose)
			{
				Console.WriteLine("Program Runs Over.");
				Console.ReadKey();
			}
		}
	}
}