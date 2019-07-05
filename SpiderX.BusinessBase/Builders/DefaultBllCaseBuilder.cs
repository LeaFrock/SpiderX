using System;
using Microsoft.Extensions.Logging;

namespace SpiderX.BusinessBase
{
	public sealed class DefaultBllCaseBuilder : IBllCaseBuilder
	{
		private readonly ILogger _logger;
		private readonly Type _subType;

		public BllCaseBuildOption Option { get; }

		public DefaultBllCaseBuilder(Type type, BllCaseBuildOption option, ILogger logger)
		{
			_subType = type ?? throw new BllCaseBuildException(new ArgumentNullException(nameof(type)));
			if (_subType.IsAbstract || _subType.IsNotPublic || !_subType.IsSubclassOf(typeof(BllBase)))
			{
				throw new BllCaseBuildException(new ArgumentException("The type cannot be Abstract/NotPublic and must be Subclass of BllBase."));
			}
			Option = option ?? BllCaseBuildOption.None;
			_logger = logger;
		}

		public BllBase Build()
		{
			BllBase bllInstance;
			try
			{
				bllInstance = (BllBase)Activator.CreateInstance(_subType, _logger, Option.RunSettings, Option.Version);
			}
			catch (Exception ex)
			{
				throw new BllCaseBuildException(ex);
			}
			return bllInstance;
		}
	}
}