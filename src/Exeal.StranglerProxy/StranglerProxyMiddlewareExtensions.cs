using Microsoft.AspNetCore.Builder;

namespace Exeal.StranglerProxy
{
    public static class StranglerProxyMiddlewareExtensions
    {
        public static IApplicationBuilder UseStranglerProxyMiddleware(this IApplicationBuilder builder)
            => builder.UseMiddleware<StranglerProxyMiddleware>();
    }
}