using System;
using System.Text;

namespace SpiderX.Tools
{
	public static class IpTool
	{
		#region IPv4

		public static uint ConvertIpToUInt32(string ip)
		{
			string[] parts = ip.Split('.');
			if (parts.Length != 4)
			{
				throw new ArgumentException("Invalid IP.");
			}
			byte a = byte.Parse(parts[0]);
			byte b = byte.Parse(parts[1]);
			byte c = byte.Parse(parts[2]);
			byte d = byte.Parse(parts[3]);
			return (uint)(a * 256 * 256 * 256 + b * 256 * 256 + c * 256 + d);
		}

		public static string ConvertUInt32ToIp(uint ip)
		{
			byte a = (byte)((ip >> 24) & 0xFF);
			byte b = (byte)((ip >> 16) & 0xFF);
			byte c = (byte)((ip >> 8) & 0xFF);
			byte d = (byte)(ip & 0xFF);
			return Ipv4PartsToString(a, b, c, d);
		}

		public static long ConvertFullIpToInt64(string host, int port)
		{
			uint ip = ConvertIpToUInt32(host);
			return ip * 65536L + port;
		}

		public static (string host, int port) ConvertInt64ToFullIp(long ip)
		{
			byte a = (byte)((ip >> 40) & 0xFF);
			byte b = (byte)((ip >> 32) & 0xFF);
			byte c = (byte)((ip >> 24) & 0xFF);
			byte d = (byte)((ip >> 16) & 0xFF);
			int port = (int)(ip & 0xFFFF);
			string host = Ipv4PartsToString(a, b, c, d);
			return (host, port);
		}

		private static string Ipv4PartsToString(byte a, byte b, byte c, byte d)
		{
			StringBuilder sb = new StringBuilder(7);
			sb.Append(a);
			sb.Append('.');
			sb.Append(b);
			sb.Append('.');
			sb.Append(c);
			sb.Append('.');
			sb.Append(d);
			string result = sb.ToString();
			return result;
		}

		#endregion IPv4
	}
}