using System;
using System.Collections.Specialized;
using System.Linq;

namespace Blueprint.Aspnet.Module.Extensions
{
    public static class NameValueCollectionExtensions
    {
        /// <summary>
        /// Partial Match two different NameValueCollection to check if one contains the other
        /// </summary>
        /// <param name="target">the target NameValueCollection to check if it contains the values from the <param name="source"></param></param>
        /// <param name="source">the source NameValueCollection to check for</param>
        /// <returns>true if <param name="target"></param> contains all keys and same values for those keys from <param name="source"></param></returns>
        public static bool Contains(this NameValueCollection target, NameValueCollection source)
        {
            return source
                .AllKeys
                .All(
                    sourceKey =>
                        target
                            .AllKeys
                            .Contains(sourceKey, StringComparer.OrdinalIgnoreCase)
                        &&
                        string.Equals(source[sourceKey], target[sourceKey], StringComparison.OrdinalIgnoreCase));
        }

        public static bool HasKey(this NameValueCollection collection, string key, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return collection.AllKeys.Any(sourceKey => string.Equals(sourceKey, key, comparison));
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