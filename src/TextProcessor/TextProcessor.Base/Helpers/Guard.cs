using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using TextProcessor.Base.Extensions;
using TextProcessor.Resource;

namespace TextProcessor.Base.Helpers
{
    public static class Guard
    {
        [DebuggerStepThrough]
        public static void IsNotEmpty<T>(string argument, Expression<Func<T>> expr)
        {
            if (string.IsNullOrEmpty((argument ?? string.Empty).Trim()))
            {
                throw new ArgumentException(Global.ArgumentCannotBeBlank.FormatWith(expr.GetParameterName()), expr.GetParameterName());
            }
        }
        [DebuggerStepThrough]
        public static void IsNotNull<T>(object argument, Expression<Func<T>> expr)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(expr.GetParameterName());
            }
        }

        [DebuggerStepThrough]
        public static void IsNotNullOrEmpty<T>(string parameter, Expression<Func<T>> expr)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                throw new ArgumentException(Global.ArgumentCannotBeNullOrEmpty.FormatWith(expr.GetParameterName()), expr.GetParameterName());
            }
        }
    }
}
