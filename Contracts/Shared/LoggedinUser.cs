using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAPI.Contracts.Shared
{
    public class LoggedinUser
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public Guid UserID { get; set; }


    }
}
