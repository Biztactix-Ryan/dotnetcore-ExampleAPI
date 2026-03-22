using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ExampleAPI.Contracts.V1;
using ExampleAPI.Contracts.V1.Auth;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExampleAPI.Controllers.V1
{

    [ApiController]
    public class AuthController : ControllerBase
    {

        [HttpPost(APIRoutes.Auth.AuthUserLogin)]
        public IActionResult Post([FromForm] UserAuthenticateRequest value)
        {
            return Ok("GoTeam!"); // Returns if User Validation Passes!
        }

        [HttpPost(APIRoutes.Auth.AuthAPILogin)]
        public IActionResult Post([FromForm] APIKeyAuthenticateRequest value)
        {
            return Ok("GoTeam!"); // no Validation
        }

    }
}
