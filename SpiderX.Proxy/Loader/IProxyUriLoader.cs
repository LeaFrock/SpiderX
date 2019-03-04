using System;

namespace SpiderX.Proxy
{
	public interface IProxyUriLoader
	{
		/// <summary>
		/// If no limits on the amount are required, set the value below 1(or just as default, Zero).
		/// </summary>
		int MaxCount { get; set; }

		Predicate<SpiderProxyUriEntity> Condition { get; set; }

		Uri[] Load();
	}
}