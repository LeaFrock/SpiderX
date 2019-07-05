using System;

namespace SpiderX.BusinessBase
{
	public sealed class EmptyBllCaseBuilder : IBllCaseBuilder
	{
		public static EmptyBllCaseBuilder Default { get; } = new EmptyBllCaseBuilder();

		public BllCaseBuildOption Option { get; }

		public BllBase Build()
		{
			throw new BllCaseBuildException(new NotImplementedException(nameof(EmptyBllCaseBuilder)));
		}
	}
}