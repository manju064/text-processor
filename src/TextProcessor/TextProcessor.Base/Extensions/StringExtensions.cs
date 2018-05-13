using System;
using System.Diagnostics;
using TextProcessor.Base.Helpers;

namespace TextProcessor.Base.Extensions
{
    /// <summary>
    /// Extension methods for string
    /// </summary>
    public static class StringExtensions
    {
        public static string ToLowerCamelCase(this string value)
        {
            return 0 < value?.Length ? char.ToLowerInvariant(value[0]) + value.Substring(1) : value;
        }


        [DebuggerStepThrough]
        public static string FormatWith(this string target, params object[] args)
        {
            Guard.IsNotEmpty(target, () => target);

            return string.Format(Helpers.Constants.CurrentCulture, target, args);
        }
    }
}
