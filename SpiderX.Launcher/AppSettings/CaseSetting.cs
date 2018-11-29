using System;

namespace SpiderX.Launcher
{
	public sealed class CaseSetting : IEquatable<CaseSetting>
	{
		public CaseSetting(string name, string[] paramsAry)
		{
			Name = name ?? throw new ArgumentNullException();
			Params = paramsAry;
		}

		public string Name { get; }

		public string[] Params { get; }

		public bool Equals(CaseSetting other)
		{
			if (Name.EndsWith("bll", StringComparison.CurrentCultureIgnoreCase))
			{
				return Name.Contains(other.Name, StringComparison.CurrentCultureIgnoreCase);
			}
			return other.Name.Contains(Name, StringComparison.CurrentCultureIgnoreCase);
		}
	}
}