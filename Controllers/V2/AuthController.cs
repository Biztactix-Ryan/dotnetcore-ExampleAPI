using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ExampleAPI.Contracts.V2;
using ExampleAPI.Contracts.V2.Auth;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExampleAPI.Controllers.V2
{

    [ApiController]
    public class AuthController : ControllerBase
    {

        [HttpPost(APIRoutes.Auth.AuthUserLogin)]
        public async Task<IActionResult> Post([FromForm] UserAuthenticateRequest value)
        {
            return Ok("GoTeam!"); // Returns if User Validation Passes!
        }

        [HttpPost(APIRoutes.Auth.AuthAPILogin)]
        public async Task<IActionResult> Post([FromForm] APIKeyAuthenticateRequest value)
        {
            return Ok("GoTeam!"); // no Validation
        }

    }
}
