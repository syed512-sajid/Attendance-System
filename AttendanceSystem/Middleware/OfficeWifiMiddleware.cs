using Microsoft.Extensions.Configuration;
using System.Net;

namespace AttendanceSystem.Middleware
{
    // Yeh middleware sirf office network (WiFi) se aane wali requests allow karta hai.
    // appsettings.json me OfficeNetwork:AllowedIpPrefixes me apne office ka IP range set karen
    // (e.g. computer par "ipconfig" chala kar IPv4 Address dekhen, us ka pehla 3 hisse yahan dalen).
    public class OfficeWifiMiddleware
    {
        private readonly RequestDelegate _next;

        public OfficeWifiMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            var enabled = configuration.GetValue<bool>("OfficeNetwork:Enabled");
            if (!enabled)
            {
                await _next(context);
                return;
            }

            var remoteIp = context.Connection.RemoteIpAddress;

            // Localhost (development/testing) hamesha allow
            if (remoteIp != null && IPAddress.IsLoopback(remoteIp))
            {
                await _next(context);
                return;
            }

            var allowedPrefixes = configuration.GetSection("OfficeNetwork:AllowedIpPrefixes").Get<string[]>() ?? Array.Empty<string>();
            var ipString = remoteIp?.MapToIPv4()?.ToString() ?? "";

            var isAllowed = allowedPrefixes.Any(prefix => ipString.StartsWith(prefix));

            if (!isAllowed)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Yeh application sirf office WiFi/network se access ho sakti hai.");
                return;
            }

            await _next(context);
        }
    }
}
