using System;

namespace SpiderX.BusinessBase
{
	public abstract class BllBase
	{
		public const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.81 Safari/537.36";

		public abstract string ClassName { get; }

		public abstract void Run(params string[] objs);

		public virtual void Run()
		{
			throw new NotImplementedException(ClassName + " Method() Not Overrided.");
		}
	}
}