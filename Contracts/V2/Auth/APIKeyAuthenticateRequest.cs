using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAPI.Contracts.V2.Auth
{
    public class APIKeyAuthenticateRequest
    {
        public string APIKey { get; set;}
        public string APIPass { get; set; }
        public string RefreshToken { get; set; }
    }
}
