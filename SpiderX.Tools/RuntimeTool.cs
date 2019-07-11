using System.Runtime.InteropServices;

namespace SpiderX.Tools
{
	public static class RuntimeTool
	{
		internal static (OSPlatform osPlatform, Architecture architecture) GetCurrentPlatform()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				return (OSPlatform.OSX, RuntimeInformation.OSArchitecture);
			}
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				return (OSPlatform.OSX, RuntimeInformation.OSArchitecture);
			}
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return (OSPlatform.OSX, RuntimeInformation.OSArchitecture);
			}
			return (OSPlatform.Create("Unknown"), RuntimeInformation.OSArchitecture);
		}
	}
}