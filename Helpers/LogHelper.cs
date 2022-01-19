using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExampleAPI.Contracts.Shared;
using Microsoft.AspNetCore.Http;
using NLog;
using NLog.StructuredLogging.Json;

namespace ExampleAPI.Helpers
{
    public class LogHelper
    {
        private readonly RequestDelegate _next;
        private readonly Logger _logger;
        public LogHelper(RequestDelegate next)
        {
            _next = next;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                if (context.User.Identity.IsAuthenticated)
                {
                  _logger.ExtendedInfo($"Request|ExampleAPI|{context.Request.Path}|",  new Dictionary<string, string>() {
                    { "API", "ExampleAPI" },
                    { "User", ((LoggedinUser)context.Items["User"]).UserID.ToString() },
                    { "IP", context.Connection.RemoteIpAddress.ToString() },
                    { "Method", context.Request.Method },
                    { "ResponseCode", context.Response.StatusCode.ToString() },
                    { "URL", context.Request.Path }});
                }
                else
                {
                    _logger.ExtendedInfo($"Request|ExampleAPI|{context.Request.Path}|", new Dictionary<string, string>() {
                    { "API", "ExampleAPI" },                    
                    { "IP", context.Connection.RemoteIpAddress.ToString() },
                    { "Method", context.Request.Method },
                    { "ResponseCode", context.Response.StatusCode.ToString() },
                    { "URL", context.Request.Path }});
                }                    
              
            }
        }


        public static void Info(NLog.Logger logger, string Message, Dictionary<string, string> Details = null)
        {
            if (Details is not null)
            {
                if (Details.ContainsKey("Message")) { Details["Message"] = Message; } else { Details.Add("Message", Message); }
                logger.ExtendedInfo(Message,Details);
            }
            else
            { logger.Info(Message); }

        }
    }
}
