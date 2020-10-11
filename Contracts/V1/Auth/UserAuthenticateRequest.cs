using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAPI.Contracts.V1
{
    public class UserAuthenticateRequest
    {
        public string User { get; set; }
        public string Pass { get; set; }
    }
}
