using System;
using System.Threading.Tasks;
using SpiderX.Extensions;

namespace SpiderX.Launcher
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			//Init GlobalSetting
			if (args.IsNullOrEmpty())
			{
				GlobalSetting.Initialize(null);
			}
			else
			{
				var caseSettings = CaseSetting.GetListByStringArgs(args);
				GlobalSetting.Initialize(caseSettings);
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
				Console.WriteLine("Program Run Over.");
				Console.ReadKey();
			}
		}
	}
}