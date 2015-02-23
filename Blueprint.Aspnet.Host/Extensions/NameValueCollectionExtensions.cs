using System;
using System.Collections.Specialized;
using System.Linq;

namespace Blueprint.Aspnet.Host.Extensions
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
    }
}