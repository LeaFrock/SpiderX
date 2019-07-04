using System;
using Microsoft.Extensions.Logging;

namespace SpiderX.BusinessBase
{
	public sealed class DefaultBllCaseBuilder : IBllCaseBuilder
	{
		private readonly ILogger _logger;

		public DefaultBllCaseBuilder(ILogger logger)
		{
			_logger = logger;
		}

		public BllBase Build(Type subType, BllCaseBuildOption option)
		{
			if (subType.IsAbstract || subType.IsNotPublic || !subType.IsSubclassOf(typeof(BllBase)))
			{
				throw new BllCaseBuildException(new ArgumentException("The type cannot be Abstract/NotPublic and must be Subclass of BllBase."));
			}
			BllBase bllInstance;
			try
			{
				bllInstance = (BllBase)Activator.CreateInstance(subType, _logger, option.RunSettings, option.Version);
			}
			catch (Exception ex)
			{
				throw new BllCaseBuildException(ex);
			}
			return bllInstance;
		}
	}
}