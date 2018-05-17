using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebUI.Infrastructure
{
    public class ExceptionLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLoggerMiddleware> _logger;


        public ExceptionLoggerMiddleware(RequestDelegate next, ILogger<ExceptionLoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
                //if (context.Response.StatusCode == StatusCodes.Status404NotFound)
                //{
                //    var statusCodeFeature = context.Features.Get<IStatusCodePagesFeature>();
                //    if (statusCodeFeature == null || !statusCodeFeature.Enabled)
                //    {
                //        // there's no StatusCodePagesMiddleware in app
                //        if (!context.Response.HasStarted)
                //        {
                //            var view = new ErrorPage(new ErrorPageModel());
                //            await view.ExecuteAsync(context);
                //        }
                //    }
                //}
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }
    }

    public static class ExceptionLoggerMiddlewareExtension
    {
        public static void UseExceptionLogger(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ExceptionLoggerMiddleware>();
        }
    }
}