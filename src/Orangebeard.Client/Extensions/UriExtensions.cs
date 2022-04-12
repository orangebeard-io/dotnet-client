using System;

namespace Orangebeard.Client.Extensions
{
    public static class UriExtensions
    {
        public static Uri Normalize(this Uri uri)
        {
            var normalizedUri = uri;

            if (!normalizedUri.ToString().EndsWith("/"))
            {
                normalizedUri = new Uri(normalizedUri + "/");
            }
            if (!normalizedUri.LocalPath.ToLowerInvariant().EndsWith("listener/v2/"))
            {
                normalizedUri = new Uri(uri + "listener/v2/");
            }

            return normalizedUri;
        }
    }
}
