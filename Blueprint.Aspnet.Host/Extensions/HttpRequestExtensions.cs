using System.IO;
using System.Web;

namespace Blueprint.Aspnet.Host.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string GetBodyString(this HttpRequest request)
        {
            var result = string.Empty;

            if (request == null)
                return result;

            using (var memStream = new MemoryStream())
            {
                if (request.InputStream.CanSeek)
                {
                    request.InputStream.Seek(0, SeekOrigin.Begin);
                }
                request.InputStream.CopyTo(memStream);

                memStream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(memStream))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }
    }
}