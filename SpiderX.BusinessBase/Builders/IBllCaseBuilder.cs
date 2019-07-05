namespace SpiderX.BusinessBase
{
	public interface IBllCaseBuilder
	{
		BllCaseBuildOption Option { get; }

		BllBase Build();
	}
}