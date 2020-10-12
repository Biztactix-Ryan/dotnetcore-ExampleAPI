using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ExampleAPI.Contracts.V1
{
    public class ExampleObjectResponse
    {
        [JsonPropertyName("id")]
        public int oid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
    }
}
