using System;
using SpiderX.BusinessBase;

namespace SpiderX.Business.CSDN
{
	public sealed class CsdnBll : BllBase
	{
		public override string ClassName => GetType().Name;

		public override void Run(params string[] objs)
		{
			Console.WriteLine(objs[0] + objs[1]);
		}
	}
}