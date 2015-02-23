using System;
using System.Collections.Generic;

namespace Blueprint.Aspnet.Host.Extensions
{
    public static class IEnumerableExtensions
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