using System;

namespace SpiderX.Proxy
{
	public sealed class WebProxyValidatorConfig
	{
		public int UseThresold { get; set; } = 100;

		public int VerifyPauseThresold { get; set; } = 300;

		public int VerifyTaskDegree { get; set; } = 100;

		public TimeSpan VerifyTaskTimeout { get; set; } = TimeSpan.FromSeconds(20);
	}
}