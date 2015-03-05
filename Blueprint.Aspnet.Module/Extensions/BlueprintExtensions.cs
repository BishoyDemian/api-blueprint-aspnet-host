using System.Collections.Specialized;
using snowcrashCLR;

namespace Blueprint.Aspnet.Module.Extensions
{
    public static class BlueprintExtensions
    {
        public static NameValueCollection Headers(this Payload payload)
        {
            var col = new NameValueCollection();
            payload
                .GetHeadersCs()
                .ForEach(h => col.Add(h.Item1, h.Item2));
            return col;
        }
    }
}