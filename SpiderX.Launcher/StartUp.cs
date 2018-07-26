using System.Text;

namespace SpiderX.Launcher
{
	public sealed class StartUp
	{
		public void Run()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		}
	}
}