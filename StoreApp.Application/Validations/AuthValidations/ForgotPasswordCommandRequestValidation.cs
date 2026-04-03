using FluentValidation;
using StoreApp.Application.CQRS.User.Command.Request;

namespace StoreApp.Application.Validations.UserValidations;

public class ForgotPasswordCommandRequestValidation : AbstractValidator<ForgotPasswordCommandRequest>
{
    public ForgotPasswordCommandRequestValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("A valid email is required.");
    }
}