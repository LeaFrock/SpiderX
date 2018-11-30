using System.Text;

namespace SpiderX.Launcher
{
	public sealed class StartUp
	{
		public static void Run()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		}
	}
}