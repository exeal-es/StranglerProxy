using Microsoft.AspNetCore.Builder;

namespace NetStandardProxy
{
    public static class ReverseProxyMiddlewareExtensions
    {
        public static IApplicationBuilder UseReverseProxyMiddleware(this IApplicationBuilder builder)
           => builder.UseMiddleware<ReverseProxyMiddleware>();
    }
}
