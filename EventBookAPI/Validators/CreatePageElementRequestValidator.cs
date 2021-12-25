using EventBookAPI.Contracts.v1.Requests;
using FluentValidation;

namespace EventBookAPI.Validators;

public class CreatePageElementRequestValidator : AbstractValidator<CreatePageElementRequest>
{
    public CreatePageElementRequestValidator()
    {
        RuleFor(request => request.Classname)
            .Matches(@"^[a-zA-Z][a-zA-Z0-9]*$")
            .WithMessage("Classname must begin with a letter and may only contain upper & lowercase letters a-z and digits 0-9");
    }
}