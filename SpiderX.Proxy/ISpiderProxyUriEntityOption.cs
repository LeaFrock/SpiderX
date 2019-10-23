using System.Linq;
using System.Linq.Expressions;

namespace SpiderX.Proxy
{
	public interface ISpiderProxyUriEntityOption
	{
		public Expression GetExpressionTree(IQueryable<SpiderProxyUriEntity> queryableData);
	}
}