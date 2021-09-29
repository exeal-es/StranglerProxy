using Microsoft.AspNetCore.Builder;

namespace NetProxy
{
    public static class ReverseProxyMiddlewareExtensions
    {
        public static IApplicationBuilder UseReverseProxyMiddleware(this IApplicationBuilder builder)
            => builder.UseMiddleware<ReverseProxyMiddleware>();
    }
}