using System.Net;

namespace SpiderX.Tools
{
	public static class WebTool
	{
		/// <summary>
		/// Use for local web debugging tools, like Fiddler etc.
		/// </summary>
		public readonly static IWebProxy Local = new WebProxy("http://localhost", 8888);
	}
}