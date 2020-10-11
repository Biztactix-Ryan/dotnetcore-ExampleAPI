using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAPI.Contracts.V2
{
    public class UserAuthenticateRequest
    {
        public string User { get; set; }
        public string Pass { get; set; }
    }
}
