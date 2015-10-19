using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModularAsp5_Beta8.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item)
        {
            foreach (var sourceItem in source)
            {
                yield return sourceItem;
            }
            yield return item;
        }
        public static IEnumerable<T> Concat<T>(this T source, IEnumerable<T> items)
        {
            yield return source;
            foreach (var item in items)
            {
                yield return item;
            }
        }
    }
}
