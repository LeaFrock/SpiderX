using System;

namespace SpiderX.Http
{
	public abstract class AppointedResponseValidator : ResponseValidatorBase
	{
		public AppointedResponseValidator(Uri uri, byte retryTimes) : base(retryTimes)
		{
			Target = uri;
		}

		public Uri Target { get; }
	}
}