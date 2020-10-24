using Microsoft.AspNetCore.Http;
using ExampleAPI.Contracts.Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
                context.Items["User"] = user;
                //ci.FindFirst("User");
                // attach user to context on successful jwt validation
                // context.Items["User"] = userService.GetById(userId);
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }

}
