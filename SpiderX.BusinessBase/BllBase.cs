namespace SpiderX.BusinessBase
{
	public abstract class BllBase
	{
		public abstract string ClassName { get; }

		public abstract void Run(params string[] objs);

		public virtual void Run()
		{
		}
	}
}