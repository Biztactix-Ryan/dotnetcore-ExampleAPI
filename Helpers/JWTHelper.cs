using Microsoft.AspNetCore.Http;
using ExampleAPI.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using NLog;

namespace ExampleAPI.Helpers
{
    public class JWTHelper
    {
        private readonly RequestDelegate _next;


        public JWTHelper(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IConfiguration configuration)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                attachUserToContext(context);
                var loggedInUser = context.Items.ContainsKey("User") ? context.Items["User"] as LoggedinUser : null;
                LogManager.GetCurrentClassLogger().Info("Request|{0}|{1}|{2}|{3}|{4}", "ExampleAPI", loggedInUser?.UserID.ToString() ?? "null", (context.Connection.RemoteIpAddress?.ToString() ?? "unknown"), context.Request.Method, context.Request.Path);
                VerifyBackendToken(context, configuration);
            }
            else
            {
                LogManager.GetCurrentClassLogger().Info("Request|{0}|{1}|{2}|{3}|{4}", "ExampleAPI", "null", (context.Connection.RemoteIpAddress?.ToString() ?? "unknown"), context.Request.Method, context.Request.Path);
            }
            await _next(context);
        }

        private void attachUserToContext(HttpContext context)
        {
            try
            {
                var ci = context.User.Identity as ClaimsIdentity;
                LoggedinUser user = new LoggedinUser();
                user.Username = ci.FindFirst("Username")?.Value;
                user.Email = ci.FindFirst("Email")?.Value;
                var userIdValue = ci.FindFirst("UserID")?.Value;
                if (!Guid.TryParse(userIdValue, out var userId))
                    return;
                user.UserID = userId;
                context.Items["User"] = user;
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
        private void VerifyBackendToken(HttpContext context, IConfiguration configuration)
        {
            context.Items["BackendAuth"] = false;
            var ci = context.User.Identity as ClaimsIdentity;
            if (ci.FindFirst("BACKEND")?.Value == "TRUE")
            {
                var tokenValue = ci.FindFirst("Token")?.Value;
                var checkValue = ci.FindFirst("Check")?.Value;
                var appNameValue = ci.FindFirst("AppName")?.Value;

                if (tokenValue == null || checkValue == null || appNameValue == null)
                    return;

                var tokenPassword = configuration["BackendAuth:TokenPassword"];
                if (string.IsNullOrEmpty(tokenPassword))
                    throw new InvalidOperationException("BackendAuth:TokenPassword is not configured in appsettings.json");

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: tokenPassword,
                salt: Convert.FromBase64String(tokenValue),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

                if (checkValue == hashed)
                {
                    context.Items["BackendAuth"] = true;
                    context.Items["BackendApp"] = appNameValue;
                }
            }



        }
    }

}
