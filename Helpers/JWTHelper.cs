using Microsoft.AspNetCore.Http;
using ExampleAPI.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                attachUserToContext(context);
                LogManager.GetCurrentClassLogger().Info("Request|{0}|{1}|{2}|{3}|{4}", "ExampleAPI", ((LoggedinUser)context.Items["User"]).UserID, context.Connection.RemoteIpAddress.ToString(), context.Request.Method, context.Request.Path);
                VerifyBackendToken(context);
            }
            else
            {
                LogManager.GetCurrentClassLogger().Info("Request|{0}|{1}|{2}|{3}|{4}", "ExampleAPI", "null", context.Connection.RemoteIpAddress.ToString(), context.Request.Method, context.Request.Path);
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
                user.UserID = Guid.Parse(ci.FindFirst("UserID")?.Value);                
                context.Items["User"] = user;
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
        private void VerifyBackendToken(HttpContext context)
        {
            context.Items["BackendAuth"] = false;
            var ci = context.User.Identity as ClaimsIdentity;
            if (ci.FindFirst("BACKEND")?.Value == "TRUE")
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: "WhyHaveAStaticToken?", // TODO: Change Backend Token
                salt: Convert.FromBase64String(ci.FindFirst("Token").Value),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

                if (ci.FindFirst("Check").Value == hashed)
                {
                    context.Items["BackendAuth"] = true;
                    context.Items["BackendApp"] = ci.FindFirst("AppName").Value;
                }
            }



        }
    }

}
