using System;
using System.Collections.Generic;

namespace Blueprint.Aspnet.Module.Extensions
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");

            if (action == null)
                throw new ArgumentNullException("action");

            foreach (var item in enumerable)
            {
                action(item);
            }

            return enumerable;
        }
    }
}