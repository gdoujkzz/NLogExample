using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLWebExample.MiddleWare
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

        public GlobalErrorHandlingMiddleware(RequestDelegate next,ILogger<GlobalErrorHandlingMiddleware> logger)
        {
            this.next = next;
            this._logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }catch(Exception ex)
            {
                var request = context.Request;
                string visitUrl = request.Path;
                string method = request.Method.ToString();
                string urlParameters = string.Empty;
                if (method == "GET") 
                {
                    urlParameters = request.QueryString.Value;
                }
                if (method == "POST")
                {
                    foreach (var item in request.Form)
                        urlParameters = urlParameters + item.Key + "=" + item.Value + "&";
                }
                string errorMsg = ex.Message;
                var logMsg = $"{visitUrl}#{method}#{urlParameters}#{errorMsg}";
                var statusCode = context.Response.StatusCode;
                var msg = $"Status Code:{statusCode},Message:{ex.Message}";
                _logger.LogError(ex, logMsg); 
                await HandleExceptionAsync(context, msg); 
            }
        }

        private static Task HandleExceptionAsync(HttpContext context,string msg)
        {
            return context.Response.WriteAsync(msg);
        }

    }

    public static class GlobalErrorHandlingMiddlewareExtension {
         public static IApplicationBuilder UseGlobalErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalErrorHandlingMiddleware>();
        }

    }
}
