using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAPI.Contracts.V1.Validators
{
    public class ExampleObjectCreateValidator : AbstractValidator<ExampleObjectCreate>
    {
        public ExampleObjectCreateValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().Matches("^[a-zA-Z]*$");
            RuleFor(x => x.LastName).NotEmpty().Matches("^[a-zA-Z]*$");
            RuleFor(x => x.Phone).Matches("^[0-9]*$");
        }
       
    }
    public class ExampleObjectUpdateValidator : AbstractValidator<ExampleObjectUpdate>
    {
        public ExampleObjectUpdateValidator()
        {
            RuleFor(x => x.FirstName).Matches("^[a-zA-Z]*$");
            RuleFor(x => x.LastName).Matches("^[a-zA-Z]*$");
            RuleFor(x => x.Phone).Matches("^[0-9]*$");
        }

    }
}
