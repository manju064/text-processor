using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace TextProcessor.Base.Extensions
{
    public static class ExpressionExtensions
    {
        public static string GetParameterName<T>(this Expression<Func<T>> parameterExpr)
        {
            var body = ((MemberExpression) parameterExpr.Body);
            return body.Member.Name;
        }
    }
}
