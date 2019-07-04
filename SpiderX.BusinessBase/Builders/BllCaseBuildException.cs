using System;

namespace SpiderX.BusinessBase
{
	public sealed class BllCaseBuildException : Exception
	{
		public BllCaseBuildException(Exception innerException) : base("Build BllCase error, please check the inner EXCEPTION.", innerException)
		{
		}
	}
}