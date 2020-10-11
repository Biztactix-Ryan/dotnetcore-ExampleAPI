using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAPI.Contracts.V1.Validators
{
    public class UserAuthenticateRequestValidator:AbstractValidator<UserAuthenticateRequest>
    {
        public UserAuthenticateRequestValidator()
        {
            RuleFor(x => x.User).NotEmpty().Matches("^[a-zA-Z0-9]*$");
            RuleFor(x => x.Pass).NotEmpty();
        }
    }
}
