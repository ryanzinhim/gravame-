using Microsoft.Extensions.Caching.Memory;
using System;

namespace gravameApi.src.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly IMemoryCache _memoryCache; 
        private static readonly string LastRequestTimestampKey = "LastRequestTimestamp";

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, IMemoryCache memoryCache)
        {
            _next = next;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_memoryCache.TryGetValue(LastRequestTimestampKey, out DateTime lastRequestTime))
            {
                var timeElapsed = DateTime.Now - lastRequestTime;

                if (timeElapsed.TotalSeconds < 20)
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteAsync("Requisição recebida muito rapidamente. Ignorando.");
                    return;
                }
            }

            _memoryCache.Set(LastRequestTimestampKey, DateTime.Now, TimeSpan.FromSeconds(20));

            context.Request.EnableBuffering();
            var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            //Console.WriteLine(body);

            context.Request.Body.Position = 0;

            await _next(context);
        }
    }
}
