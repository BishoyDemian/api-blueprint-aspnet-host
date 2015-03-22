using System;
using System.Collections.Specialized;
using System.Linq;

namespace Blueprint.Aspnet.Module.Extensions
{
    internal static class NameValueCollectionExtensions
    {
        /// <summary>
        /// Match two different NameValueCollection to check if <param name="a">a</param> contains <param name="b">b</param>
        /// </summary>
        /// <returns>true if <param name="a"></param> contains all keys and equal values for those keys from <param name="b"></param></returns>
        public static bool Contains(this NameValueCollection a, NameValueCollection b)
        {
            if (a == null)
                throw new ArgumentNullException("a");

            if (b == null)
                throw new ArgumentNullException("b");

            if (a.Count == 0)
                return true;

            var sourceKeys = a.AllKeys;
            var targetKeys = b.AllKeys;

            return targetKeys
                .All(
                    targetKey =>
                        sourceKeys.Contains(targetKey, StringComparer.OrdinalIgnoreCase)
                        &&
                        string.Equals(a[targetKey], b[targetKey], StringComparison.OrdinalIgnoreCase));
        }

        public static bool HasKey(this NameValueCollection collection, string key, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return collection.AllKeys.Any(sourceKey => string.Equals(sourceKey, key, comparison));
        }

        public static string ValueOrDefault(this NameValueCollection collection, string key)
        {
            return (collection.GetValues(key) ?? new string[]{}).FirstOrDefault();
        }


        public static NameValueCollection Except(this NameValueCollection collection, string key)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            var copy = new NameValueCollection(collection);
            copy.Remove(key);
            return copy;
        }

    }
}