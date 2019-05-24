using System.Text;

namespace SpiderX.Launcher
{
	internal sealed class Preparation
	{
		public static void JustDoIt()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		}
	}
}