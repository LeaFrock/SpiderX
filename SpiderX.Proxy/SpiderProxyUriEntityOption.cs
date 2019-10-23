using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxyUriEntityOption : ISpiderProxyUriEntityOption
	{
		/// <summary>
		/// IP类型（0Http 1Https），默认值255，表示不配置
		/// </summary>
		public byte Category { get; set; } = 255;

		/// <summary>
		/// 匿名度（0透明 1普匿 2混淆 3高匿），默认值255，表示不配置
		/// </summary>
		public byte AnonymityDegree { get; set; } = 255;

		/// <summary>
		/// 默认值0，表示不配置
		/// </summary>
		public int ResponseMilliseconds { get; set; }

		Expression ISpiderProxyUriEntityOption.GetExpressionTree(IQueryable<SpiderProxyUriEntity> queryableData)
		{
			Expression left, right;
			Type entityType = typeof(SpiderProxyUriEntity);
			var tempExpressions = new LinkedList<Expression>();
			ParameterExpression pe = Expression.Parameter(entityType, "e");
			if (Category < 255)
			{
				left = Expression.Property(pe, entityType.GetProperty(nameof(SpiderProxyUriEntity.Category)));
				right = Expression.Constant(Category);
				tempExpressions.AddLast(Expression.Equal(left, right));
			}
			if (AnonymityDegree < 255)
			{
				left = Expression.Property(pe, entityType.GetProperty(nameof(SpiderProxyUriEntity.AnonymityDegree)));
				right = Expression.Constant(AnonymityDegree);
				tempExpressions.AddLast(Expression.Equal(left, right));
			}
			if (ResponseMilliseconds > 0)
			{
				left = Expression.Property(pe, entityType.GetProperty(nameof(SpiderProxyUriEntity.ResponseMilliseconds)));
				right = Expression.Constant(ResponseMilliseconds);
				tempExpressions.AddLast(Expression.LessThanOrEqual(left, right));
			}
			var node = tempExpressions.First;
			if (node is null)
			{
				return null;
			}
			Expression predicateBody = node.Value;
			while (node.Next != null)
			{
				predicateBody = Expression.AndAlso(predicateBody, node.Next.Value);
				node = node.Next;
			}
			return Expression.Call(typeof(Queryable), "Where", new Type[] { queryableData.ElementType }, queryableData.Expression,
				Expression.Lambda<Func<SpiderProxyUriEntity, bool>>(predicateBody, pe));
		}
	}
}