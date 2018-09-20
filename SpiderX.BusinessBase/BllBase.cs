namespace SpiderX.BusinessBase
{
	public abstract class BllBase
	{
		public abstract string ClassName { get; }

		public abstract void Run(params string[] args);

		public virtual void Run()
		{
		}
	}
}