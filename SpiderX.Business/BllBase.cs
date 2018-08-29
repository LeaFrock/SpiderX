namespace SpiderX.Business
{
	public abstract class BllBase
	{
		public abstract string ClassName { get; }

		public abstract void Run();
	}
}