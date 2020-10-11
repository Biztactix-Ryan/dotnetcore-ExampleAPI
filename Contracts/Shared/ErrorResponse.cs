using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAPI.Contracts.Shared
{
    public class ErrorResponse
    {
        public ErrorResponse() { }

        public ErrorResponse(ErrorModel error)
        {
            Errors.Add(error);
        }

        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();


    }
}
