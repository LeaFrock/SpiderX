using System;

namespace SpiderX.BusinessBase
{
	public interface IBllCaseBuilder
	{
		BllBase Build(Type subType, BllCaseBuildOption option);
	}
}