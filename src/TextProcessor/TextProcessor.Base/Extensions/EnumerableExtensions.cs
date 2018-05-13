using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TextProcessor.Base.Enums;

namespace TextProcessor.Base.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            if (list != null)
            {
                foreach (T x in list)
                {
                    action(x);
                }
            }
        }
        public static async Task ForEachAsync<T>(this IEnumerable<T> list, Func<T, Task> func)
        {
            foreach (var value in list)
            {
                await func(value);
            }
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> source, OrderBy direction) 
        {
            if (direction == Enums.OrderBy.Desc)
                return source.OrderByDescending(x => x);
            else
                return source.OrderBy(x => x);
        }
    }
}
